
namespace BrønnApi.Company.DTOs
{
    public class StatisticsReportDto
    {
        public Dictionary<string, int> StatusCounts { get; set; } = new();
        public Dictionary<string, double> OrganizationFormDistributionPercentage { get; set; } = new();
        public EmployeeCountDistributionDto EmployeeDistribution { get; set; } = new();
    }

    public class EmployeeCountDistributionDto
    {
        public int Zero { get; set; }
        public int OneToNine { get; set; }
        public int TenToFortyNine { get; set; }
        public int FiftyPlus { get; set; }
    }
}