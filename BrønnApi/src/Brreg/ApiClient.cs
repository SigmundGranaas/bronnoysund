
using System.Net;
using BrønnApi.Brreg.DTOs;
using Microsoft.Extensions.Options;

namespace BrønnApi.Brreg;

public interface IBrregApiClient
{
    Task<Enhet?> GetEnhetAsync(int orgNr);
}

public class BrregApiSettings
{
    public string BaseUrl { get; init; } = "https://data.brreg.no/enhetsregisteret/api/";
}

public class BrregApiClient : IBrregApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BrregApiClient> _logger;

    public BrregApiClient(HttpClient httpClient, IOptions<BrregApiSettings> settings, ILogger<BrregApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(settings.Value.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<Enhet?> GetEnhetAsync(int orgNr)
    {
        try
        {
            var response = await _httpClient.GetAsync($"enheter/{orgNr}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Organization number {OrgNr} not found in Brreg.", orgNr);
                return null;
            }
            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var contents = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Request is malformed or not accepted: {contents}", contents);
                return null;
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Enhet>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching data from Brreg API for {OrgNr}.", orgNr);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing Brreg API response for {OrgNr}.", orgNr);
            return null;
        }
    }
}