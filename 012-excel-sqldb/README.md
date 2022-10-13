# Dealing with Multiple Cultures in Data Processing

When processing data in multiple languages, it is necessary to be mindful of the data "Culture".

For example in an Excel column, one column may contain "textual data" that needs to be sorted in one culture, but the numeric column may need to be converted into "decimal numbers" that will need to use another culture.

In this solution we are taking an Excel file, loading it into a DataTable and then using Entity Framework to save the data into a SQL database.

- when displaying DataTable to the Console, we are using the "ar-SA" culture.
- when converting balance columns, we are using the "en-US" culture. (if you try to use ar-SA to do TryParse, it will fail.)

```
using System.Globalization;

Decimal.TryParse("-21518.51", NumberStyles.Currency, new CultureInfo("ar-SA"), out var result);
Console.WriteLine(result); // Output: 0

```

## Setting up Entity Framework

- enable `Unicode` for the `Description` column (using Collation `Latin1_General_100_BIN2_UTF8`)  
- set all columns to use `varchar`.

```
dotnet ef migrations add InitDB  --project .\SqlDbLib\SqlDbLib.csproj --startup-project .\Excel2Sql\Excel2Sql.csproj

-- manually edit Migrations scripts and change 'nvarchar' to 'varchar'

dotnet ef migrations bundle --project .\SqlDbLib\SqlDbLib.csproj --startup-project .\Excel2Sql\Excel2Sql.csproj
.\efbundle.exe
dotnet ef migrations add AddCurrency  --project .\SqlDbLib\SqlDbLib.csproj --startup-project .\Excel2Sql\Excel2Sql.csproj
dotnet ef migrations bundle --project .\SqlDbLib\SqlDbLib.csproj --startup-project .\Excel2Sql\Excel2Sql.csproj
.\efbundle.exe
```

## Test Run

- should see the DataTable displayed in `ar-SA` culture older  
- should see strings are successfully converted and saved to the SQL database as `decimal` values in numeric format.  
- if all currency values are converted successfully, all records should satisfy:

`[BalanceCarryOver] + [PeriodDebtReporting] - [PeriodCreditReporting] = [CumulativeBalance]`

```
dotnet run --project .\Excel2Sql\Excel2Sql.csproj
```