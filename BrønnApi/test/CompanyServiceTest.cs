using System.Globalization;
using System.Text;
using BrønnApi.Brreg;
using BrønnApi.Company;
using BrønnApi.Csv;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace BrønnApi.test;

[TestClass]
public class CompanyProcessingTest
{
    private CsvCompanyParser? _csvParser;
    private ICompanyProcessingService? _companyService;
    private IBrregApiClient? _brregApiClient;
    
    [TestInitialize]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        var httpClient = new HttpClient();
        var brregSettings = Options.Create(new BrregApiSettings
        {
            BaseUrl = "https://data.brreg.no/enhetsregisteret/api/"
        });
        
        var brregLogger = loggerFactory.CreateLogger<BrregApiClient>();
        _brregApiClient = new BrregApiClient(httpClient, brregSettings, brregLogger);
        
        var parserLogger = loggerFactory.CreateLogger<CsvCompanyParser>();
        _csvParser = new CsvCompanyParser(parserLogger);
        
        var companyLogger = loggerFactory.CreateLogger<CompanyService>();
        _companyService = new CompanyService(_brregApiClient, companyLogger);
    }
    
    [TestMethod]
    public async Task TestProcessCompaniesFromBrreg()
    {
        if (_csvParser == null || _companyService == null || _brregApiClient == null)
        {
            Assert.Inconclusive("Service setup failed");
            return;
        }
        
        var inputFileName = "firmaer.csv";
        var inputFilePath = Path.Combine("test", "TestData", inputFileName);
        var outputFileName = "firmaer_output.csv";
        var outputFilePath = Path.Combine("test", "TestData", outputFileName);
        
        try
        {
            Assert.IsTrue(File.Exists(inputFilePath), $"Test file {inputFilePath} must exist");
            
            List<CSVCompanyRecord> companyRecords;
            await using (var fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                companyRecords = _csvParser.ParseCsv(fileStream);
            }
            
            // Limit the number of records for testing to avoid making too many API calls
            var limitedCompanies = companyRecords.Take(10).ToList();
            
            var processed = await _companyService.ProcessCompaniesAsync(limitedCompanies);
            Assert.IsNotNull(processed, "Processed companies should not be null");
            Assert.AreEqual(limitedCompanies.Count, processed.Count, "All companies should be processed");
            
            await using (var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8))
            await using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            }))
            {
                await csv.WriteRecordsAsync(processed);
            }
            
            foreach (var company in processed)
            {
                Assert.IsNotNull(company.OrgNo, "Organization number should not be null");
                Assert.IsNotNull(company.Status, "Status should not be null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during test: {ex.Message}"); 
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }
}