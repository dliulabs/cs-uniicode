using Microsoft.EntityFrameworkCore.Migrations;

namespace SqlDbLib.Migrations {
    public partial class InitDB : Migration {
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable (
                name: "TrialBalance",
                columns : table => new {
                    Id = table.Column<long> (type: "bigint", nullable : false)
                        .Annotation ("SqlServer:Identity", "1, 1"),
                        Code = table.Column<string> (type: "varchar(100)", unicode : false, maxLength : 100, nullable : false),
                        GLAccount = table.Column<string> (type: "varchar(100)", unicode : false, maxLength : 100, nullable : false),
                        Description = table.Column<string> (type: "varchar(100)", maxLength : 100, nullable : false, collation: "Latin1_General_100_BIN2_UTF8"),
                        Currency = table.Column<string> (type: "varchar(100)", unicode : false, maxLength : 100, nullable : false),
                        BalanceCarryover = table.Column<decimal> (type: "decimal(18,2)", precision : 18, scale : 2, nullable : true),
                        PeriodDebtReporting = table.Column<decimal> (type: "decimal(18,2)", precision : 18, scale : 2, nullable : true),
                        PeriodCreditReporting = table.Column<decimal> (type: "decimal(18,2)", precision : 18, scale : 2, nullable : true),
                        CumulativeBalance = table.Column<decimal> (type: "decimal(18,2)", precision : 18, scale : 2, nullable : true)
                },
                constraints : table => {
                    table.PrimaryKey ("PK_TrialBalance", x => x.Id);
                });
        }

        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable (
                name: "TrialBalance");
        }
    }
}