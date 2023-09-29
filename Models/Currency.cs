using System.Text.Json.Serialization;

public class Currency
{
    // This is the ISO4217 code
    [JsonPropertyName("code")]
    public required string CurrencyCode { get; set; }
    [JsonPropertyName("numeric")]
    public required string NumericCurrencyCode { get; set; }
    [JsonPropertyName("name")]
    public required string CurrencyLongName { get; set; }
    [JsonPropertyName("shortName")]
    public required string CurrencyShortName { get; set; }
    [JsonPropertyName("decimals")]
    public int NumOfDecimals { get; set; }
    [JsonPropertyName("symbol")]
    public string? CurrencySymbol { get; set; }
}