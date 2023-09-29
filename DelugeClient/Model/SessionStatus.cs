using System.Text.Json.Serialization;

public class SessionStatus
{
    [JsonPropertyName("upload_rate")]
    public decimal UploadRate { get; set; }

    [JsonPropertyName("download_rate")]
    public decimal DownloadRate { get; set; }

    [JsonPropertyName("num_peers")]
    public int NumPeers { get; set; }
}