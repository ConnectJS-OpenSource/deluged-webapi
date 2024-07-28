using Microsoft.AspNetCore.Mvc;

namespace DelugeClient
{
    public class Endpoints
    {
        public static DelugeWebClient client;
        public static string deluged_url;
        public static string deluged_pass;
        public static void Register(WebApplication app, string root = "/api")
        {
            deluged_url = app.Configuration.GetValue<string>("deluged-host");
            deluged_pass = app.Configuration.GetValue<string>("deluged-password");

            Console.WriteLine($"Connecting Deluged at {deluged_url}");

            client = new DelugeWebClient(deluged_url);

            app.MapGroup(root).MapGet($"/login", async () =>
            {
                await client.LoginAsync(deluged_pass);
                return new { message = "OK" };
            });

            app.MapGroup(root).MapGet("/torrents", async () =>
            {
                var results = await client.GetTorrentsStatusAsync();
                return results.OrderBy(m => m.IsFinished);
            });

            app.MapGroup(root).MapGet("/torrent/{Id}/{action}", async (string Id, TorrentAction action) =>
            {
                await client.TorrentActionsAsync(Id, action);
                return new { message = "OK" };
            });

            app.MapGroup(root).MapGet("/torrent/{Id}", async (string Id) =>
            {
                var results = await client.GetTorrentsStatusAsync(Id);
                return results.FirstOrDefault();
            });

            app.MapGroup(root).MapPost("/torrent", async (NewTorrent torrent) =>
            {
                await client.AddTorrentMagnetAsync(torrent.Path, new TorrentOptions
                {
                    DownloadLocation = DelugeWebClient.PathMappings[torrent.Type]
                });

                return new { message = "OK" };
            });

            app.MapGroup(root).MapGet("/execute/{method}", async ([FromRoute] string method, [FromBody] object[] param) =>
            {
                var response = await client.ExecuteRaw(method, param);
                return response;
            });
        }
    }
}
