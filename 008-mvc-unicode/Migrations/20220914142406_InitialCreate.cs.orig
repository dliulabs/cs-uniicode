using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcMovie.Migrations {
    public partial class InitialCreate : Migration {
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable (
                name: "Movie",
                columns : table => new {
                    Id = table.Column<int> (type: "int", nullable : false)
                        .Annotation ("SqlServer:Identity", "1, 1"),
                        Title = table.Column<string> (type: "varchar(100)", maxLength : 100, nullable : false, collation: "Latin1_General_100_BIN2_UTF8"),
                        ReleaseDate = table.Column<DateTime> (type: "datetime2", nullable : false),
                        Price = table.Column<decimal> (type: "decimal(18,2)", nullable : false),
                        Genre = table.Column<string> (type: "varchar(100)", unicode : false, maxLength : 100, nullable : false),
                        Rating = table.Column<string> (type: "varchar(100)", unicode : false, maxLength : 100, nullable : false)
                },
                constraints : table => {
                    table.PrimaryKey ("PK_Movie", x => x.Id);
                });
        }

        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable (
                name: "Movie");
        }
    }
}