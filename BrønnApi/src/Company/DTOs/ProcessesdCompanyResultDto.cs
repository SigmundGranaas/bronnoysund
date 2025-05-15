namespace BrønnApi.Company.DTOs
{
    public class ProcessesdCompanyResultDto
    {
        public List<ProcessedCompanyDto> Companies { get; init; } = new();
        public StatisticsReportDto Statistics { get; init; } = new ();
    }
}