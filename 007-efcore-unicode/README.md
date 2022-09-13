# EF Core 6 support for Unicode

[Unicode and UTF-8](https://docs.microsoft.com/en-us/ef/core/providers/sql-server/columns#unicode-and-utf-8)

SQL Server has two column types for storing textual data: nvarchar(x) and varchar(x); these have traditionally been used to hold Unicode data in the UTF-16 encoding and non-Unicode data, respectively. SQL Server 2019 [introduced](https://docs.microsoft.com/en-us/sql/relational-databases/collations/collation-and-unicode-support#utf8) the ability to store UTF-8 Unicode data in varchar(x) columns.

Unfortunately, this does not currently work out-of-the-box with EF Core's SQL Server provider. To map a string property to a varchar(x) column, the Fluent or Data Annotation API is typically used to disable Unicode ([see these docs](https://docs.microsoft.com/en-us/ef/core/modeling/entity-properties#unicode)). While this causes the correct column type to be created, it also makes EF Core send database parameters in a way which is incompatible with UTF-8 data: DbType.AnsiString is used (signifying non-Unicode data), but DbType.String is needed to properly send Unicode data.

To store UTF-8 data in SQL Server, follow these steps:

Configure the collation for the property with one of SQL Server's UTF-8 collations; these have a UTF8 suffix ([see the docs on collations](https://docs.microsoft.com/en-us/ef/core/providers/sql-server/columns#unicode-and-utf-8:~:text=see%20the%20docs%20on%20collations)).
Do not disable Unicode on the property; this will cause EF Core to create an nvarchar(x) column.
Edit the migrations and manually set the column type to varchar(x) instead.

## Generate Migration script

- Prepare your project to build successfully

  * make sure the data column is annotated with `[Unicode(true)]`, and
  * the Fluent API is configured to use Unicode + `_UTF8` collation:

```
  protected override void OnModelCreating(ModelBuilder builder) {
        builder.Entity<Entities.MyTable>(p => {
            p.HasKey(k => k.Id);
            p.Property(prop => prop.Param).IsUnicode(true).UseCollation("Latin1_General_100_BIN2_UTF8").HasMaxLength(100);

        });
    }
```

- run the dotnet-ef tool to generate a migration script

```
dotnet ef migrations add InitDB --project ./EFCoreDbLib/EFCoreDbLib.csproj --startup-project ./EFCoreConsole/EFCoreConsole.csproj
```

## Manually edit script to remove `nvarchar`

The default EF Core 6.0 will generate Unicode columns using `nvarchar`, manually modify them to use `varchar`

```
diff ./EFCoreDbLib/Migrations/20220913201152_InitDB.cs.orig ./EFCoreDbLib/Migrations/20220913201152_InitDB.cs
14c14
<                         Param = table.Column<string>(type: "nvarchar(100)", maxLength : 100, nullable : true, collation: "Latin1_General_100_BIN2_UTF8")
---
>                         Param = table.Column<string>(type: "varchar(100)", maxLength : 100, nullable : true, collation: "Latin1_General_100_BIN2_UTF8")
```

## Deploy data model to Sql Server

```
dotnet ef migrations bundle --project ./EFCoreDbLib/EFCoreDbLib.csproj --startup-project ./EFCoreConsole/EFCoreConsole.csproj
./efbundle
```

once deployed, you can verify the collation of the data colum:

```
SELECT name, collation_name FROM sys.columns WHERE name = N'Param';
```

## Test Run the program

```
dotnet run --project ./EFCoreConsole/EFCoreConsole.csproj
``` 

You can verify in the SQL Database that all Unicode text are captured successfully in the data tabel.