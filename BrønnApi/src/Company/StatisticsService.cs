using BrønnApi.Company.DTOs;

namespace BrønnApi.Company;

public class StatisticsService
{
    public StatisticsReportDto CalculateStatistics(List<ProcessedCompanyDto> companies)
        {
            if (companies.Count == 0)
            {
                return new StatisticsReportDto
                {
                    StatusCounts = new Dictionary<string, int>(),
                    OrganizationFormDistributionPercentage = new Dictionary<string, double>(),
                    EmployeeDistribution = new EmployeeCountDistributionDto()
                };
            }

            // Choosing to keep data field nullable in case of malformed responses was not a good idea.
            // Now everything has to be checked and verified, which increases complexity a lot.
            var statusCounts = companies
                .GroupBy(c => c.Status ?? "Ukjent")
                .ToDictionary(g => g.Key, g => g.Count());

            var validOrgForms = companies
                .Where(c => !string.IsNullOrEmpty(c.OrganisasjonsformKode) && c.OrganisasjonsformKode != "N/A" && c.OrganisasjonsformKode != "Ukjent")
                .ToList();
            
            var totalCompaniesWithValidOrgForm = validOrgForms.Count;
                
            var organizationFormDistributionPercentage = validOrgForms
                .GroupBy(c => c.OrganisasjonsformKode)
                .ToDictionary(
                    g => g.Key!, 
                    g => totalCompaniesWithValidOrgForm > 0 ? Math.Round((double)g.Count() / totalCompaniesWithValidOrgForm * 100, 1) : 0
                );

            var employeeDistribution = new EmployeeCountDistributionDto
            {
                Zero = companies.Count(c => c.AntallAnsatte == 0),
                OneToNine = companies.Count(c => c.AntallAnsatte.HasValue && c.AntallAnsatte > 0 && c.AntallAnsatte <= 9),
                TenToFortyNine = companies.Count(c => c.AntallAnsatte.HasValue && c.AntallAnsatte >= 10 && c.AntallAnsatte <= 49),
                FiftyPlus = companies.Count(c => c.AntallAnsatte.HasValue && c.AntallAnsatte >= 50)
            };

            return new StatisticsReportDto
            {
                StatusCounts = statusCounts,
                OrganizationFormDistributionPercentage = organizationFormDistributionPercentage,
                EmployeeDistribution = employeeDistribution
            };
        }
}