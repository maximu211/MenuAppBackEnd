using MenuApp.API.Extensions;
using MenuApp.BLL.Configuration;
using MenuApp.BLL.Services.UserService;
using MenuApp.DAL.Configurations;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MenuApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.ConfigureProjectSettings(configuration);
builder.Services.AddMongoDBConfiguration(configuration);

builder.Services.AddUserService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
