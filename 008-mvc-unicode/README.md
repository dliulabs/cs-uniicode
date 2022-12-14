# Configuring Unicode for EF Entities 

Step 1: In you DbContext, configure a default to disable `Unicode` so that the default column data type will be `varchar` (instead of `nvarchar`)

Step 2: Turn on `Unicode(true)` for the data entity colunns and set collation to one of the UTF8 collations.

## Turn on EntityFramework logging to see the SQL command generated by the code

- Do this only in non-production environments. Do not do this in production environments.

1. In the appsettings.json file, add EF logging:

```
 "Logging": {
    "LogLevel": {
      ...
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
```

2. In the Startup.cs or Program.cs, turn on sentive data logging so that you can see the parameter values:

Since you will be logging unicode text to console, you will need to add this line to your Startup.cs or Program.cs file:

```
Console.OutputEncoding = System.Text.Encoding.UTF8;

builder.Services.AddDbContext<MvcMovieContext> (options => {
    options.UseSqlServer (builder.Configuration.GetConnectionString ("MvcMovieContext"));
    options.EnableSensitiveDataLogging (); // to see parameter values
});
```

## Test Run

```
dotnt run
```

Test Editing a Movie and change a Title to have Unicode text.

You should see a log similar to the following:

```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (40ms) [Parameters=[@p5='1', @p0='Romantic Comedy' (Nullable = false) 
(Size = 100) (DbType = AnsiString), @p1='7.99' (Precision = 18) (Scale = 2), @p2='R' (Nullable 
= false) (Size = 100) (DbType = AnsiString), @p3='1989-02-12T00:00:00.0000000', @p4='当哈利碰上
莎莉' (Nullable = false) (Size = 100)], CommandType='Text', CommandTimeout='30']
      SET NOCOUNT ON;
      UPDATE [Movie] SET [Genre] = @p0, [Price] = @p1, [Rating] = @p2, [ReleaseDate] = @p3, [Title] = @p4
      WHERE [Id] = @p5;
      SELECT @@ROWCOUNT;
```

Where the `Genre` parameter will be sent as `DbType = AnsiString`, while the `Title` parameter will not be sent as `AnsiString` (but as `String` - by default).