using d2;
using d2.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore;

/// <summary>Extensions for converting Entity Framework Core relational models to D2.</summary>
public static class D2ModelExtensions
{
  /// <summary>Creates a D2 SQL-table diagram from a context's relational model.</summary>
  /// <typeparam name="TContext">The Entity Framework Core context type.</typeparam>
  /// <param name="context">The context whose model is converted.</param>
  /// <param name="configure">Optional diagram-generation settings.</param>
  /// <returns>A diagram containing the model's tables, columns, constraints, and relationships.</returns>
  public static D2Diagram ToD2Diagram<TContext>(
    this TContext context,
    Action<D2SchemaOptions>? configure = null)
    where TContext : DbContext
  {
    if (context is null) throw new ArgumentNullException(nameof(context));
    return context.Model.ToD2Diagram(configure);
  }

  /// <summary>Creates a D2 SQL-table diagram from an Entity Framework Core relational model.</summary>
  /// <param name="model">The relational model to convert.</param>
  /// <param name="configure">Optional diagram-generation settings.</param>
  /// <returns>A diagram containing the model's tables, columns, constraints, and relationships.</returns>
  public static D2Diagram ToD2Diagram(
    this IModel model,
    Action<D2SchemaOptions>? configure = null)
  {
    if (model is null) throw new ArgumentNullException(nameof(model));

    var options = new D2SchemaOptions();
    configure?.Invoke(options);
    options.Validate();
    return CreateD2Diagram(model, options);
  }

  internal static D2Diagram CreateD2Diagram(IModel model, D2SchemaOptions options)
  {
    var tables = model.GetRelationalModel().Tables
      .OrderBy(table => table.Schema, StringComparer.Ordinal)
      .ThenBy(table => table.Name, StringComparer.Ordinal)
      .ToList();

    var tableReferences = tables.ToDictionary(
      table => table,
      table => GetTableReference(table, options));

    var duplicateReference = tableReferences.Values
      .GroupBy(reference => reference, StringComparer.Ordinal)
      .FirstOrDefault(group => group.Count() > 1);
    if (duplicateReference is not null)
    {
      throw new InvalidOperationException(
        $"Multiple database tables map to the D2 key '{duplicateReference.Key}'. " +
        "Enable database schemas to disambiguate them.");
    }

    var statements = new List<D2Statement>();
    foreach (var table in tables)
    {
      var sqlTable = new D2SqlTable(tableReferences[table]);
      foreach (var column in table.Columns
        .OrderByDescending(column => table.PrimaryKey?.Columns.Contains(column) == true)
        .ThenBy(column => column.Name, StringComparer.Ordinal))
      {
        sqlTable.Add(new D2SqlColumn(
          column.Name,
          GetColumnType(column, options),
          GetColumnConstraints(table, column)));
      }

      statements.Add(sqlTable);
    }

    if (options.IncludeForeignKeyConnections)
    {
      statements.AddRange(CreateForeignKeyConnections(tables, tableReferences, options));
    }

    return new D2Diagram(statements);
  }

  private static string GetTableReference(ITable table, D2SchemaOptions options)
    => options.IncludeDatabaseSchemas && !string.IsNullOrWhiteSpace(table.Schema)
      ? $"{table.Schema}.{table.Name}"
      : table.Name;

  private static string GetColumnType(IColumn column, D2SchemaOptions options)
  {
    if (!options.IncludeNullability)
    {
      return column.StoreType;
    }

    return $"{column.StoreType} {(column.IsNullable ? "NULL" : "NOT NULL")}";
  }

  private static D2SqlConstraint[] GetColumnConstraints(ITable table, IColumn column)
  {
    var constraints = new List<D2SqlConstraint>();

    if (table.PrimaryKey?.Columns.Contains(column) == true)
    {
      constraints.Add(D2SqlConstraint.PrimaryKey);
    }

    if (table.ForeignKeyConstraints.Any(foreignKey => foreignKey.Columns.Contains(column)))
    {
      constraints.Add(D2SqlConstraint.ForeignKey);
    }

    var belongsToUniqueConstraint = table.UniqueConstraints.Any(
      constraint => !ReferenceEquals(constraint, table.PrimaryKey) && constraint.Columns.Contains(column));
    var belongsToUniqueIndex = table.Indexes.Any(
      index => index.IsUnique && index.Columns.Contains(column));
    if (belongsToUniqueConstraint || belongsToUniqueIndex)
    {
      constraints.Add(D2SqlConstraint.Unique);
    }

    return constraints.ToArray();
  }

  private static IEnumerable<D2Statement> CreateForeignKeyConnections(
    IEnumerable<ITable> tables,
    IReadOnlyDictionary<ITable, string> tableReferences,
    D2SchemaOptions options)
  {
    foreach (var table in tables)
    {
      foreach (var foreignKey in table.ForeignKeyConstraints.OrderBy(
        foreignKey => foreignKey.Name,
        StringComparer.Ordinal))
      {
        var columnCount = Math.Min(foreignKey.Columns.Count, foreignKey.PrincipalColumns.Count);
        for (var index = 0; index < columnCount; index++)
        {
          var dependent = $"{tableReferences[table]}.{foreignKey.Columns[index].Name}";
          var principal = $"{tableReferences[foreignKey.PrincipalTable]}.{foreignKey.PrincipalColumns[index].Name}";
          var label = options.IncludeForeignKeyNames ? foreignKey.Name : null;
          yield return new D2Connection(dependent, principal, Direction.To, label);
        }
      }
    }
  }
}
