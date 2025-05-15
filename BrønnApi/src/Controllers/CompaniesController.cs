using Microsoft.AspNetCore.Mvc;
using BrønnApi.Csv;
using BrønnApi.Company;
using BrønnApi.Company.DTOs;
using System.Text;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace BrønnApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly CsvCompanyParser _csvParser;
        private readonly ICompanyProcessingService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            CsvCompanyParser csvParser,
            ICompanyProcessingService companyService,
            ILogger<CompaniesController> logger)
        {
            _csvParser = csvParser;
            _companyService = companyService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(ProcessesdCompanyResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadCompanyCsv(IFormFile file)
        {
            // There's too much logic in the controller. Ideally this should be moved to a dedicated service for orchestrating the parsing and processing of the data
            if (file.Length == 0)
            {
                _logger.LogWarning("Upload attempt with no file or empty file.");
                return BadRequest("Please upload a valid CSV file.");
            }

            if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase) &&
                !file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Upload attempt with invalid file type: {ContentType}, Filename: {FileName}", file.ContentType, file.FileName);
                return BadRequest("Invalid file type. Please upload a CSV file.");
            }

            _logger.LogInformation("Received file: {FileName}, Size: {Length} bytes, ContentType: {ContentType}", file.FileName, file.Length, file.ContentType);

            List<CSVCompanyRecord> parsedRecords;
            try
            {
                await using (var stream = file.OpenReadStream())
                {
                    parsedRecords = _csvParser.ParseCsv(stream);
                }

                if (parsedRecords.Count == 0)
                {
                    _logger.LogWarning("CSV parsing resulted in no records for file: {FileName}", file.FileName);
                    return BadRequest("The CSV file is empty or could not be parsed correctly. Ensure it has a header and data rows matching 'FirmaNavn' and 'OrgNr'.");
                }
                _logger.LogInformation("Successfully parsed {RecordCount} records from {FileName}", parsedRecords.Count, file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing CSV file: {FileName}", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while parsing the CSV file.");
            }

            List<ProcessedCompanyDto> processedCompanies;
            try
            {
                processedCompanies = await _companyService.ProcessCompaniesAsync(parsedRecords);
                _logger.LogInformation("Successfully processed {CompanyCount} companies from {FileName}", processedCompanies.Count, file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing company data from file: {FileName}", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing company data.");
            }

            StatisticsReportDto statistics;
            try
            {
                var statsService = new StatisticsService();
                statistics = statsService.CalculateStatistics(processedCompanies);
                _logger.LogInformation("Successfully calculated statistics for {FileName}", file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating statistics for file: {FileName}", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while calculating statistics.");
            }
            
            var result = new ProcessesdCompanyResultDto
            {
                Companies = processedCompanies,
                Statistics = statistics
            };

            return Ok(result);
        }

        [HttpPost("upload/csv")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadAndReturnCsv(IFormFile file)
        {
            if (file.Length == 0)
            {
                _logger.LogWarning("Upload attempt with no file or empty file.");
                return BadRequest("Please upload a valid CSV file.");
            }

            if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase) &&
                !file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Upload attempt with invalid file type: {ContentType}, Filename: {FileName}", file.ContentType, file.FileName);
                return BadRequest("Invalid file type. Please upload a CSV file.");
            }

            _logger.LogInformation("Received file for CSV processing: {FileName}, Size: {Length} bytes", file.FileName, file.Length);

            List<CSVCompanyRecord> parsedRecords;
            try
            {
                await using var stream = file.OpenReadStream();
                parsedRecords = _csvParser.ParseCsv(stream);

                if (parsedRecords.Count == 0)
                {
                    _logger.LogWarning("CSV parsing resulted in no records for file: {FileName}", file.FileName);
                    return BadRequest("The CSV file is empty or could not be parsed correctly.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing CSV file: {FileName}", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while parsing the CSV file.");
            }

            List<ProcessedCompanyDto> processedCompanies;
            try
            {
                processedCompanies = await _companyService.ProcessCompaniesAsync(parsedRecords);
                _logger.LogInformation("Successfully processed {CompanyCount} companies for CSV export", processedCompanies.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing company data from file: {FileName}", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing company data.");
            }

            // Should be handled in a CSV writer, but it's too late for that now ;)
            byte[] csvBytes;
            try
            {
                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true
                });

                await csv.WriteRecordsAsync(processedCompanies);
                await writer.FlushAsync();
                csvBytes = memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating CSV output");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the CSV file.");
            }
            
            return File(csvBytes, "text/csv", "firmaer_output.csv");
        }
    }
}