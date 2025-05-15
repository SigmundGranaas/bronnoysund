namespace BrønnApi.Company.DTOs;

public class ProcessedCompanyDto
{
    public string? OrgNo { get; set; }
    public string? FirmaNavn { get; set; }
    public string? Status { get; set; }
    public int? AntallAnsatte { get; set; }
    public string? OrganisasjonsformKode { get; set; }
    public string? Naeringskode { get; set; }
}