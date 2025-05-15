namespace BrønnApi.Company.DTOs;

using Brreg;
using BrønnApi.Brreg.DTOs;

public static class BrregDataMapper
{
    public static ProcessedCompanyDto ToProcessedCompanyDto(int orgNr, Enhet? brregEnhet)
    {
        if (brregEnhet == null)
        {
            // Handle cases where the orgNr was not found in Brreg or an API call failed.
            // We'll keep the record, but clearly annotate that it failed.
            // Failed records can be sorted out later
            return new ProcessedCompanyDto
            {
                OrgNo = orgNr.ToString(), 
                FirmaNavn = "N/A (Organisasjonsnummer ikke funnet eller feil ved oppslag)",
                Status = CompanyStatus.Feil, 
                AntallAnsatte = null,
                OrganisasjonsformKode = "N/A",
                Naeringskode = "N/A"
            };
        }

        string status;
        if (!string.IsNullOrEmpty(brregEnhet.Slettedato))
        {
            status = CompanyStatus.Slettet;
        }
        else if (brregEnhet.Konkurs)
        {
            status = CompanyStatus.Konkurs;
        }
        else if (brregEnhet.UnderAvvikling || brregEnhet.UnderTvangsavviklingEllerTvangsopplosning)
        {
            status = CompanyStatus.UnderAvvikling;
        }
        else
        {
            status = CompanyStatus.Aktiv;
        }

        return new ProcessedCompanyDto
        {
            OrgNo = brregEnhet.Organisasjonsnummer,
            FirmaNavn = brregEnhet.Navn,
            Status = status,
            AntallAnsatte = brregEnhet.AntallAnsatte,
            OrganisasjonsformKode = brregEnhet.Organisasjonsform?.Kode, 
            Naeringskode = brregEnhet.Naeringskode1?.Kode           
        };
    }
}