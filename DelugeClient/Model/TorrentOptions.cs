using System.Text.Json.Serialization;

public class TorrentOptions
{
    [JsonPropertyName("download_location")]
    public required String DownloadLocation { get; set; }
}