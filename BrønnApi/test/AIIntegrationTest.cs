using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrønnApi.Csv;

namespace BrønnApi.test;

[TestClass]
public class CsvParserTest
{
    private CsvCompanyParser? _parser;
    
    [TestInitialize]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        var parserLogger = loggerFactory.CreateLogger<CsvCompanyParser>();
        _parser = new CsvCompanyParser(parserLogger);
    }
    
    [TestMethod]
    public void TestWithRegisteredCsvFile()
    {
        // Skip if setup failed
        if (_parser == null)
        {
            Assert.Inconclusive("Parser setup failed");
            return;
        }
        
        var fileName = "firmaer.csv";
        var filePath = Path.Combine("test","TestData", fileName);
        
        Console.WriteLine($"Testing with file: {filePath}");
        
        try
        {
            Assert.IsTrue(File.Exists(filePath), $"Test file {filePath} must exist");
            
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var records = _parser.ParseCsv(fileStream);
            
            Console.WriteLine($"\nParsing Summary:");
            Console.WriteLine($"Successfully parsed rows: {records.Count}");
            
            var sampleRecords = records.Take(5).ToList();
            if (sampleRecords.Count > 0)
            {
                Console.WriteLine("\nSample of parsed rows:");
                foreach (var record in sampleRecords)
                {
                    Console.WriteLine($"OrgNr: {record.OrgNr}, FirmaNavn: {record.FirmaNavn}");
                }
                
                if (records.Count > 5)
                {
                    Console.WriteLine($"... and {records.Count - 5} more");
                }
            }
            
            Assert.IsTrue(records.Count > 0, "Should successfully parse at least some rows");
            
            foreach (var record in sampleRecords)
            {
                Assert.IsNotNull(record.OrgNr, "Organization number should not be empty");
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