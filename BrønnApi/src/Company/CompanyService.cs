using System.Collections.Concurrent;
using BrønnApi.Brreg;
using BrønnApi.Company.DTOs;
using BrønnApi.Csv;

namespace BrønnApi.Company;

public interface ICompanyProcessingService
{
    Task<List<ProcessedCompanyDto>> ProcessCompaniesAsync(List<CSVCompanyRecord> companyInputs);
}

public class CompanyService : ICompanyProcessingService
{
    private readonly IBrregApiClient _brregApiClient;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(IBrregApiClient brregApiClient, ILogger<CompanyService> logger)
    {
        _brregApiClient = brregApiClient;
        _logger = logger;
    }

    public async Task<List<ProcessedCompanyDto>> ProcessCompaniesAsync(List<CSVCompanyRecord> companyInputs)
    {
        // Remove duplicates by OrgNr
        var uniqueCompanies = companyInputs
            .GroupBy(c => c.OrgNr)
            .Select(g => g.First())
            .ToList();
            
        _logger.LogInformation("Processing {TotalCount} companies (Where {UniqueCount} are unique)", 
            companyInputs.Count, uniqueCompanies.Count);

        var processedCompany = new ConcurrentBag<ProcessedCompanyDto>();
        
        // Process in batches with max parallelism of 20. It seems reasonable?
        // This increases the complexity a fair bit, but it also speeds the process significantly.
        var semaphore = new SemaphoreSlim(20);
        var tasks = uniqueCompanies.Select(async rawInput => 
        {
            try
            {
                await semaphore.WaitAsync();
                try
                {
                    var brregEnhet = await _brregApiClient.GetEnhetAsync(rawInput.OrgNr);
                    var processedDto = BrregDataMapper.ToProcessedCompanyDto(rawInput.OrgNr, brregEnhet);

                    
                    // Annotate companies that had issues with processing
                    if (processedDto.Status == CompanyStatus.Feil && brregEnhet == null && !string.IsNullOrWhiteSpace(rawInput.FirmaNavn))
                    {
                        processedDto.FirmaNavn = $"{rawInput.FirmaNavn} (OrgNr: {rawInput.OrgNr} - Brreg lookup failed)";
                    }
                    else if (processedDto.Status == CompanyStatus.Feil && brregEnhet == null && string.IsNullOrWhiteSpace(rawInput.FirmaNavn))
                    {
                        processedDto.FirmaNavn = $"N/A (OrgNr: {rawInput.OrgNr} - Brreg lookup failed)";
                    }

                    processedCompany.Add(processedDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during processing for OrgNr {OrgNr}. Input FirmaNavn: {InputFirmaNavn}", 
                        rawInput.OrgNr, rawInput.FirmaNavn);
                    
                    var errorDto = BrregDataMapper.ToProcessedCompanyDto(rawInput.OrgNr, null);
                    if(!string.IsNullOrWhiteSpace(rawInput.FirmaNavn))
                    {
                        errorDto.FirmaNavn = $"{rawInput.FirmaNavn} (OrgNr: {rawInput.OrgNr} - Processing error)";
                    }
                    else
                    {
                        errorDto.FirmaNavn = $"N/A (OrgNr: {rawInput.OrgNr} - Processing error)";
                    }
                    processedCompany.Add(errorDto);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception processing OrgNr {OrgNr}", rawInput.OrgNr);

                var errorDto = BrregDataMapper.ToProcessedCompanyDto(rawInput.OrgNr, null);
                errorDto.FirmaNavn = $"N/A (OrgNr: {rawInput.OrgNr} - Critical error)";
                processedCompany.Add(errorDto);
            }
        });

        await Task.WhenAll(tasks);
        
        _logger.LogInformation("Completed processing of {Count} companies", processedCompany.Count);
        
        return processedCompany.ToList();
    }
}