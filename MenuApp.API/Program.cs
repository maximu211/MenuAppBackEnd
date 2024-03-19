using MenuApp.API.Extensions.Configuration;
using MenuApp.API.Extensions.ServiceExtensions;
using MenuApp.BLL.Workers;
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

builder.Services.AddHostedService<CodesCleanUpWorker>();
builder.Services.AddHostedService<UsersCleanUpWorker>();

builder.Services.AddUserService();
builder.Services.AddConfirmationCodesService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder =>
        builder
            .WithOrigins("http://localhost:7296")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
