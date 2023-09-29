
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DelugeConfig
{
    [JsonPropertyName("max_download_speed")]
    public double MaxDownloadSpeed { get; set; }

    [JsonPropertyName("max_upload_speed")]
    public double MaxUploadSpeed { get; set; }

    [JsonPropertyName("torrentfiles_location")]
    public String TorrentFilesLocation { get; set; }

    [JsonPropertyName("move_completed_path")]
    public String MoveCompletedPath { get; set; }

    [JsonPropertyName("max_connections")]
    public int MaxConnections { get; set; }
}