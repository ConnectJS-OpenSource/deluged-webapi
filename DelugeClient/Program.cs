using DelugeClient;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(args);

builder.Services.AddCors(s =>
{
    s.AddDefaultPolicy(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();

var client = new DelugeWebClient(app.Configuration.GetValue<string>("url"), app.Configuration.GetValue<string>("password"));
await client.LoginAsync();

app.MapGet("/", () => new { message = "Deluge Web Client UP and Running" });

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

app.MapGet("/execute/{method}", async ([FromRoute]string method, [FromBody]object[] param) =>
{
    var response = await client.ExecuteRaw(method, param);
    return response;
});



app.Run();