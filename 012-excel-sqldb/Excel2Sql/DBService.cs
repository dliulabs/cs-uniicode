using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SqlDbLib;

namespace Excel2Sql;

public class DbService {
	// IConfiguration _configuration;
	ApplicationDbContext _dbContext;
	static CultureInfo _UsCulture = new CultureInfo ("en-US");

	// public DbService(IConfiguration configuration, ApplicationDbContext dbContext) {
	// _configuration = configuration;
	public DbService (ApplicationDbContext dbContext) {
		_dbContext = dbContext;
	}

	static Decimal? TryParseCurrency (string value) {
		if (Decimal.TryParse (value, NumberStyles.Currency, _UsCulture, out var number))
			return number;
		else
			return null;
	}
	public void DataTable2Sql (DataTable dt) {
		var tb = dt.AsEnumerable ()
			// .OrderBy (x => x.Field<string> ("نص قصير"))
			.Select ((a, i) => {

				// Decimal.TryParse (a.Field<string> ("ترحيل الرصيد"), NumberStyles.Currency, _UsCulture, out var balanceCarryover);

				return new SqlDbLib.Entities.TrialBalance {
					Code = a.Field<string> ("رمز"),
						GLAccount = a.Field<string> ("حساب G/L"),
						Description = a.Field<string> ("نص قصير"),
						Currency = a.Field<string> ("عملة"),
						BalanceCarryover = TryParseCurrency (a.Field<string> ("ترحيل الرصيد")),
						PeriodDebtReporting = TryParseCurrency (a.Field<string> ("فترةإعدادتقارير مدين")),
						PeriodCreditReporting = TryParseCurrency (a.Field<string> ("فترة تقرير الدائن")),
						CumulativeBalance = TryParseCurrency (a.Field<string> ("رصيد تراكمي")),
				};
			});
		_dbContext.TrialBalance.AddRange (tb);
		_dbContext.SaveChanges ();
	}
}