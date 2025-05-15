using System.Text.Json.Serialization;

namespace BrønnApi.Brreg.DTOs;

public class Enhet
{
    [JsonPropertyName("organisasjonsnummer")]
    public string Organisasjonsnummer { get; set; } = string.Empty;

    [JsonPropertyName("navn")]
    public string Navn { get; set; } = string.Empty;

    [JsonPropertyName("organisasjonsform")]
    public Organisasjonsform? Organisasjonsform { get; set; }

    [JsonPropertyName("naeringskode1")]
    public Naeringskode? Naeringskode1 { get; set; }

    [JsonPropertyName("antallAnsatte")]
    public int AntallAnsatte { get; set; }

    [JsonPropertyName("konkurs")]
    public bool Konkurs { get; set; }

    [JsonPropertyName("underAvvikling")]
    public bool UnderAvvikling { get; set; }

    [JsonPropertyName("underTvangsavviklingEllerTvangsopplosning")]
    public bool UnderTvangsavviklingEllerTvangsopplosning { get; set; }

    [JsonPropertyName("slettedato")]
    public string? Slettedato { get; set; }
}

public class Organisasjonsform
{
    [JsonPropertyName("kode")]
    public string Kode { get; set; } = string.Empty;

    [JsonPropertyName("beskrivelse")]
    public string Beskrivelse { get; set; } = string.Empty;
}

public class Naeringskode
{
    [JsonPropertyName("kode")]
    public string Kode { get; set; } = string.Empty;

    [JsonPropertyName("beskrivelse")]
    public string Beskrivelse { get; set; } = string.Empty;
}