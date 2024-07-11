using DelugeClient;
using Microsoft.AspNetCore.Mvc;

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

app.UseCors();

Endpoints.Register(app);

app.Run();