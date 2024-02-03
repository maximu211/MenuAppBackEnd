using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MenuApp.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json");
var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings").Get<DBSettings>();
builder.Services.AddSingleton(new DBContext(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName));

// Ðו÷סענאצ³ÿ IUsersRepository עא ימדמ נואכ³חאצ³¿
builder.Services.AddSingleton<IUsersRepository, UserRepository>();


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
