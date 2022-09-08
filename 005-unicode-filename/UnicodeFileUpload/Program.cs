// using System.Globalization;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

[assembly: RootNamespace("UnicodeFileUpload")]


var builder = WebApplication.CreateBuilder(args);
/* builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
}); */

Console.OutputEncoding = System.Text.Encoding.UTF8;
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

logger.LogInformation("Look! I can log non-Western characters: 你好！");
await app.RunAsync();
