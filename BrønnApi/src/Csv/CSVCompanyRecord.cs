using CsvHelper.Configuration;

namespace BrønnApi.Csv;


public class CSVCompanyRecord
{
    public string FirmaNavn { get; init; }
    public int OrgNr { get; init; }
}

public sealed class NormalCSVCompanyRecordMap : ClassMap<CSVCompanyRecord>
{
    public NormalCSVCompanyRecordMap()
    {
        Map(m => m.FirmaNavn).Name("FirmaNavn");
        Map(m => m.OrgNr).Name("OrgNr");
    }
}

public sealed class ReversedCSVCompanyRecordMap : ClassMap<CSVCompanyRecord>
{
    public ReversedCSVCompanyRecordMap()
    {
        Map(m => m.FirmaNavn).Name("OrgNr");
        Map(m => m.OrgNr).Name("FirmaNavn");
    }
}


