using System.Text.Json.Serialization;

internal class WebRequestMessage
{
    [JsonPropertyName("id")]
    public int RequestId { get; set; }

    [JsonPropertyName("method")]
    public String Method { get; set; }

    [JsonPropertyName("params")]
    public List<Object> Params { get; set; }

    public WebRequestMessage(int requestId, String method, params object[] parameters)
    {
        RequestId = requestId;
        Method = method;
        Params = new List<Object>();

        if (parameters != null) Params.AddRange(parameters);

    }
}