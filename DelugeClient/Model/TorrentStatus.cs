using System.Text.Json.Serialization;

public class TorrentStatus
{
    [JsonPropertyName("comment")]
    public String Comment { get; set; }

    [JsonPropertyName("hash")]
    public String Hash { get; set; }

    [JsonPropertyName("paused")]
    public Boolean Paused { get; set; }

    [JsonPropertyName("ratio")]
    public double Ratio { get; set; }

    [JsonPropertyName("message")]
    public String Message { get; set; }

    [JsonPropertyName("name")]
    public String Name { get; set; }

    [JsonPropertyName("is_seed")]
    public Boolean IsSeed { get; set; }

    [JsonPropertyName("is_finished")]
    public bool IsFinished { get; set; }

    [JsonPropertyName("queue")]
    public int Queue { get; set; }

    [JsonPropertyName("save_path")]
    public String SavePath { get; set; }

    [JsonPropertyName("progress")]
    public decimal Progress { get; set; }

    [JsonPropertyName("download_payload_rate")]
    public decimal DownloadRate { get; set; }

    public override string ToString()
    {
        return Name;
    }
}