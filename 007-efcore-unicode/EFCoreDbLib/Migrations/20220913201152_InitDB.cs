using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCoreDbLib.Migrations {
    public partial class InitDB : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "MyTable",
                columns : table => new {
                    Id = table.Column<long>(type: "bigint", nullable : false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                        Param = table.Column<string>(type: "varchar(100)", maxLength : 100, nullable : true, collation: "Latin1_General_100_BIN2_UTF8")
                },
                constraints : table => {
                    table.PrimaryKey("PK_MyTable", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "MyTable");
        }
    }
}