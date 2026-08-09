namespace d2.EntityFrameworkCore;

/// <summary>Controls how an Entity Framework Core relational model is represented in D2.</summary>
public sealed class D2SchemaOptions
{
  /// <summary>
  /// Gets or sets the generated <c>.d2</c> file path. Relative paths are resolved
  /// from the host content root.
  /// </summary>
  public string OutputPath { get; set; } = "database-schema.d2";

  /// <summary>Gets or sets whether table keys include their database schema.</summary>
  public bool IncludeDatabaseSchemas { get; set; } = true;

  /// <summary>Gets or sets whether column types include <c>NULL</c> or <c>NOT NULL</c>.</summary>
  public bool IncludeNullability { get; set; } = true;

  /// <summary>Gets or sets whether foreign keys are rendered as column-to-column connections.</summary>
  public bool IncludeForeignKeyConnections { get; set; } = true;

  /// <summary>Gets or sets whether foreign-key constraint names label their connections.</summary>
  public bool IncludeForeignKeyNames { get; set; }

  internal void Validate()
  {
    if (string.IsNullOrWhiteSpace(OutputPath))
    {
      throw new InvalidOperationException("A D2 schema output path is required.");
    }
  }
}
