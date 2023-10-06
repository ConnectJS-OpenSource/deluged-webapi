using DelugeClient;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(args).AddEnvironmentVariables();

builder.Services.AddCors(s =>
{
    s.AddDefaultPolicy(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

builder.Services.AddHostedService<BGService>();

var app = builder.Build();

app.UseCors();

Endpoints.Register(app);

app.Run();