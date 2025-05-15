using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace BrønnApi.Csv;

public class CsvCompanyParser
{
    private readonly ILogger<CsvCompanyParser> _logger;
    private const string CsvDelimiter = ";";

    public CsvCompanyParser(ILogger<CsvCompanyParser> logger)
    {
        _logger = logger;
    }

    public List<CSVCompanyRecord> ParseCsv(Stream csvStream)
    {
        // Due to the original CSV file having the header reversed?
        // Either I am stupid, or the header was reversed intentionally in the csv
        // We will have to try both ways...
        var records = TryParseCsv(csvStream, false);
        
        // If parsing failed or found no valid records, try with reversed headers
        if (records.Count == 0)
        {
            _logger.LogInformation("Initial parsing yielded no records, attempting with reversed field mapping");
            csvStream.Seek(0, SeekOrigin.Begin);
            records = TryParseCsv(csvStream, true);
        }

        _logger.LogInformation("Parsed {Count} company records successfully", records.Count);
        return records;
    }

    private List<CSVCompanyRecord> TryParseCsv(Stream csvStream, bool reverseMapping)
    {
        var records = new List<CSVCompanyRecord>();
        
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = CsvDelimiter,
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.Trim(),
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim
        };
        
        using (var reader = new StreamReader(csvStream, Encoding.UTF8, true, 1024, true))
        using (var csv = new CsvReader(reader, csvConfig))
        {
            try
            {
                if (reverseMapping)
                {
                    csv.Context.RegisterClassMap<ReversedCSVCompanyRecordMap>();
                    _logger.LogInformation("Using reversed field mapping");
                }
                else
                {
                    csv.Context.RegisterClassMap<NormalCSVCompanyRecordMap>();
                    _logger.LogInformation("Using normal field mapping");
                }

                if (csv.Read() && csv.ReadHeader())
                {
                    var headers = csv.HeaderRecord != null ? string.Join(", ", csv.HeaderRecord) : "unknown";
                    _logger.LogInformation("Successfully read CSV header: {Headers}", headers);
                }
                else
                {
                    _logger.LogError("CSV is empty or has no header");
                    return records;
                }

                while (csv.Read())
                {
                    try
                    {
                        var record = csv.GetRecord<CSVCompanyRecord>();
                        if (record.OrgNr > 0 && !string.IsNullOrWhiteSpace(record.FirmaNavn))
                        {
                            records.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error parsing record at row {RowNumber}", csv.Context.Parser?.Row);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing CSV with {MappingType} mapping", 
                    reverseMapping ? "reversed" : "normal");
            }
        }

        return records;
    }
}
