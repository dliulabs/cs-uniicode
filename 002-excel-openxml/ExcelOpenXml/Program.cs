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
                var (app, core) = excelMetadata.ProcessExcelAsync().GetAwaiter().GetResult();
                Console.WriteLine ("Author: {0}", core?.Creator);
                Console.WriteLine ("Created: {0}", core?.Created);
                Console.WriteLine ("Last Modified: {0}", core?.Modified);
                Console.WriteLine ("Application: {0}", app?.Application);
                Console.WriteLine ("App Version: {0}", app?.AppVersion);
            }
            catch (Exception ex)
            {
                host.Services.GetRequiredService<ILogger<Program>>()
                    .LogError(ex, "Unable to load branches from GitHub.");
            }

        }

    }
}