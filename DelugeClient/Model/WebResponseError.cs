using System.Text.Json.Serialization;

internal class WebResponseError
{
    [JsonPropertyName("messag")]
    public String Message { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }
}