using System.Security.Cryptography.X509Certificates;
using MenuApp.API.Extensions.Configuration;
using MenuApp.API.Extensions.ServiceExtensions;
using MenuApp.BLL.Workers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var loger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(loger);

builder.Services.AddControllers();

builder.Services.ConfigureProjectSettings(configuration);
builder.Services.AddMongoDBConfiguration(configuration);

builder.Services.AddHostedService<UsersCleanUpWorker>();

builder.Services.AddUserService();
builder.Services.AddConfirmationCodesService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.WebHost.UseUrls("https://localhost:5001");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder =>
        builder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
    );
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
