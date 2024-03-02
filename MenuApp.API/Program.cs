using MenuApp.API.Extensions;
using MenuApp.BLL.Configuration;
using MenuApp.BLL.Services.UserService;
using MenuApp.DAL.Configurations;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MenuApp.DAL.Repositories;
using static MenuApp.BLL.Services.UserService.UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json");
var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings").Get<DBSettings>();
builder.Services.AddSingleton(
    new DBContext(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName)
);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

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
