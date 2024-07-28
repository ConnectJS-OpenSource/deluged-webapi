using DelugeClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(s =>
{
    s.AddDefaultPolicy(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

DelugeWebClient.PathMappings = new Dictionary<string, string>()
 {
    {"movie", builder.Configuration.GetValue<string>("movie-path") },
    {"tv", builder.Configuration.GetValue<string>("tv-path") }
};


builder.Services.AddHostedService<BGService>();

var app = builder.Build();



Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSpa(config =>
{
    config.Options.DefaultPage = "/index.html";
});

app.MapGet("/health", (IConfiguration configuration) =>
{
    return new
    {
        paths = DelugeClient.DelugeWebClient.PathMappings,
        host = configuration.GetValue<string>("deluged-host")
    };
});




app.UseCors();

Endpoints.Register(app);

app.Run();