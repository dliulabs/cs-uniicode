using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;
using Excel2Ndjson;
using Microsoft.Extensions.Localization;

[assembly: RootNamespace("Excel2Ndjson")]

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddTransient<SAXParsing>();
    })
    .Build();

try
{
    var excelParsing = host.Services.GetRequiredService<SAXParsing>();
    await excelParsing.ProcessExcelAsync();
}
catch (Exception ex)
{
    host.Services.GetRequiredService<ILogger<Program>>()
        .LogError(ex, "Unable to load required service.");
}