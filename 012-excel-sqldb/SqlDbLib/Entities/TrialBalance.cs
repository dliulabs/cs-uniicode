using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SqlDbLib.Entities;

public class TrialBalance {
	public long Id { get; set; }
	public string Code { get; set; }
	public string GLAccount { get; set; }

	[Unicode (true)]
	public string Description { get; set; }

	public string Currency { get; set; }

	[Precision (18, 2)]
	public decimal? BalanceCarryover { get; set; }

	[Precision (18, 2)]
	public decimal? PeriodDebtReporting { get; set; }

	[Precision (18, 2)]
	public decimal? PeriodCreditReporting { get; set; }

	[Precision (18, 2)]
	public decimal? CumulativeBalance { get; set; }
}