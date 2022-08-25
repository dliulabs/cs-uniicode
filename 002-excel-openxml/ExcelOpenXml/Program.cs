using System.Text;
// using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;


namespace ExcelOpenXml
{
    public class Program
    {

        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient(); 
                    services.AddTransient<ExcelMetadata>();
                })
                .Build();


            try
            {
                var excelMetadata = host.Services.GetRequiredService<ExcelMetadata>();
                var result = excelMetadata.ProcessExcelAsync().GetAwaiter().GetResult();
                Console.WriteLine ("Excel Version: {0}", result?.AppVersion);
            }
            catch (Exception ex)
            {
                host.Services.GetRequiredService<ILogger<Program>>()
                    .LogError(ex, "Unable to load branches from GitHub.");
            }

        }

    }
}