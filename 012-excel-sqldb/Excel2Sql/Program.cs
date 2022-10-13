using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlDbLib;

namespace Excel2Sql {
	public class Program {
		public static async Task Main () {
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			var host = new HostBuilder ()
				.ConfigureAppConfiguration (config => {
					config
						.SetBasePath (Directory.GetCurrentDirectory ())
						.AddJsonFile ("appsettings.json");
				})
				.ConfigureServices ((context, services) => {
					services.AddHttpClient ();
					services.AddTransient<SAXParsing> ();
					services.AddTransient<DbService> ();
					var cns = context.Configuration.GetConnectionString ("MySqlContext");
					Console.WriteLine (cns);
					services.AddDbContext<ApplicationDbContext> (options => {
						options.UseSqlServer (cns);
					});
				})
				.Build ();

			try {
				var excelParsing = host.Services.GetRequiredService<SAXParsing> ();
				DataTable dt = await excelParsing.ProcessExcelAsync ();

				var arCulture = new CultureInfo ("ar-SA");
				var usCulture = new CultureInfo ("en-US");

				var data = dt.AsEnumerable ()
					.OrderBy (x => x.Field<string> ("نص قصير"), StringComparer.Create (arCulture, false))
					.Select ((a, i) => {
						Decimal.TryParse (a.Field<string> ("ترحيل الرصيد"), NumberStyles.Currency, usCulture, out var balanceCarryover);

						return new {
							Code = a.Field<string> ("رمز"),
								GLAccount = a.Field<string> ("حساب G/L"),
								Description = a.Field<string> ("نص قصير"),
								Currency = a.Field<string> ("عملة"),
								BalanceCarryOver = balanceCarryover,
						};
					});

				foreach (var rec in data) {
					Console.WriteLine ($"Code: {rec.Code}, gl_account: {rec.GLAccount}, desc: {rec.Description}, carryover = {rec.BalanceCarryOver}");
				}

				var dbService = host.Services.GetRequiredService<DbService> ();
				dbService.DataTable2Sql (dt);
			} catch (Exception ex) {
				host.Services.GetRequiredService<ILogger<Program>> ()
					.LogError (ex, "Unable to load required service.");
			}
		}
	}
}