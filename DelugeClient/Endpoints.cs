using Microsoft.AspNetCore.Mvc;

namespace DelugeClient
{
    public class Endpoints
    {
        public static void Register(WebApplication app)
        {
            var deluged_url = app.Configuration.GetValue<string>("deluged-host") ?? app.Configuration.GetValue<string>("DELUGE_URL");
            var deluged_pass = app.Configuration.GetValue<string>("deluged-password") ?? app.Configuration.GetValue<string>("DELUGE_PASS");

            Console.WriteLine($"Connecting Deluged at {deluged_url}");

            var client = new DelugeWebClient(deluged_url);

            app.MapGet("/", () => new { message = "Deluge Web Client UP and Running" });

            app.MapGet("/login", async () =>
            {
                await client.LoginAsync(deluged_pass);
                return new { message = "OK" };
            });

            app.MapGet("/torrents", async () =>
            {
                var results = await client.GetTorrentsStatusAsync();
                return results.OrderBy(m => m.IsFinished);
            });

            app.MapGet("/torrent/{Id}/{action}", async (string Id, TorrentAction action) =>
            {
                await client.TorrentActionsAsync(Id, action);
                return new { message = "OK" };
            });

            app.MapGet("/torrent/{Id}", async (string Id) =>
            {
                var results = await client.GetTorrentsStatusAsync(Id);
                return results.FirstOrDefault();
            });

            app.MapPost("/torrent", async (NewTorrent torrent) =>
            {
                await client.AddTorrentMagnetAsync(torrent.Path, new TorrentOptions
                {
                    DownloadLocation = client.PathMappings[torrent.Type]
                });

                return new { message = "OK" };
            });

            app.MapGet("/execute/{method}", async ([FromRoute] string method, [FromBody] object[] param) =>
            {
                var response = await client.ExecuteRaw(method, param);
                return response;
            });
        }
    }
}
