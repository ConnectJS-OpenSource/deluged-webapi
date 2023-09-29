using System.Text.Json.Serialization;

internal class WebResponseMessage<T>
{

    [JsonPropertyName("id")]
    public int ResponseId { get; set; }

    [JsonPropertyName("result")]
    public T Result { get; set; }

    [JsonPropertyName("error")]
    public WebResponseError Error { get; set; }
}