using System.Text;
// using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;

namespace ExcelParsing
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient(); 
                    services.AddTransient<SaxParsing>();
                })
                .Build();
            
             try
            {
                var excelParsing = host.Services.GetRequiredService<SaxParsing>();
                await excelParsing.ProcessExcelAsync();
            }
            catch (Exception ex)
            {
                host.Services.GetRequiredService<ILogger<Program>>()
                    .LogError(ex, "Unable to load required service.");
            }
        }
    }
}