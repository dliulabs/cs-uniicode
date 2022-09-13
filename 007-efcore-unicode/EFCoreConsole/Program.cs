using System.IO;
using System.Linq;
using EFCoreConsole;
using EFCoreDbLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

[assembly : RootNamespace("EFCoreConsole")]

var host = new HostBuilder()
    .ConfigureAppConfiguration(config => {
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
    })
    .ConfigureServices((context, services) => {
        var cns = context.Configuration.GetConnectionString("AdventureWorks");
        // Console.WriteLine(cns);
        services.AddTransient<DbService>();
        services.AddDbContext<ApplicationDbContext>(options => {
            options.UseSqlServer(cns);
        });
    })
    .Build();

try {
    var dbService = host.Services.GetRequiredService<DbService>();
    dbService.SeedData();
    dbService.ListMyTable();
} catch (Exception ex) {
    Console.WriteLine(ex);
}