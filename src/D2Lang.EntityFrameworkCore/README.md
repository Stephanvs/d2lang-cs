# D2Lang.EntityFrameworkCore

Generate a D2 SQL-table diagram directly from an Entity Framework Core relational model.

## Install

```bash
dotnet add package D2Lang.EntityFrameworkCore
```

The package references `d2lang-cs` transitively. It supports EF Core applications targeting .NET 8 or .NET 10.

## Automatic generation

Chain `AddD2Schema` after the normal context registration:

```csharp
builder.Services
    .AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Database")))
    .AddD2Schema<AppDbContext>("docs/database-schema.d2");
```

The file is generated when the host starts. Relative paths use the host content root. The generator reads EF Core's in-memory relational model, so it does not connect to the database. Files are replaced only when their content changes.

The output contains database tables, columns, provider store types, nullability, primary keys, unique columns, foreign-key constraints, and column-to-column foreign-key connections. Use the independently installed D2 CLI to render the generated source:

```bash
d2 docs/database-schema.d2 docs/database-schema.svg
```

Generation can be customized during registration:

```csharp
.AddD2Schema<AppDbContext>("docs/database-schema.d2", options =>
{
    options.IncludeDatabaseSchemas = true;
    options.IncludeNullability = true;
    options.IncludeForeignKeyConnections = true;
    options.IncludeForeignKeyNames = false;
});
```

For applications that do not use the .NET generic host, create the diagram directly:

```csharp
var diagram = dbContext.ToD2Diagram();
var source = diagram.ToString();
```
