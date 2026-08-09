using System.Collections;

namespace d2;

/// <summary>A SQL-table constraint understood by D2.</summary>
public sealed record D2SqlConstraint
{
  /// <summary>A primary-key constraint.</summary>
  public static readonly D2SqlConstraint PrimaryKey = new("primary_key");
  /// <summary>A foreign-key constraint.</summary>
  public static readonly D2SqlConstraint ForeignKey = new("foreign_key");
  /// <summary>A uniqueness constraint.</summary>
  public static readonly D2SqlConstraint Unique = new("unique");

  /// <summary>The D2 constraint value.</summary>
  public string Value { get; }

  private D2SqlConstraint(string value)
  {
    Value = value;
  }

  /// <summary>Creates a custom SQL constraint value.</summary>
  /// <param name="value">The constraint text shown by D2.</param>
  /// <returns>A constraint containing the supplied value.</returns>
  public static D2SqlConstraint Custom(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("A SQL constraint cannot be null, empty, or whitespace.", nameof(value));
    }

    _ = D2Writer.String(value);
    return new D2SqlConstraint(value);
  }

  /// <inheritdoc />
  public override string ToString() => Value;
}

/// <summary>A typed column in a <see cref="D2SqlTable"/>.</summary>
public sealed record D2SqlColumn : D2Statement
{
  /// <summary>The column key.</summary>
  public string Name { get; }
  /// <summary>The SQL type displayed by D2.</summary>
  public string Type { get; }
  /// <summary>The column's constraints.</summary>
  public IReadOnlyList<D2SqlConstraint> Constraints { get; }

  /// <summary>Creates a SQL column.</summary>
  /// <param name="name">The column key.</param>
  /// <param name="type">The SQL type displayed by D2.</param>
  /// <param name="constraints">The column constraints.</param>
  public D2SqlColumn(string name, string type, params D2SqlConstraint[] constraints)
  {
    _ = D2Writer.Identifier(name);
    if (string.IsNullOrWhiteSpace(type))
    {
      throw new ArgumentException("A SQL column type cannot be null, empty, or whitespace.", nameof(type));
    }
    if (constraints is null)
    {
      throw new ArgumentNullException(nameof(constraints));
    }
    if (constraints.Any(constraint => constraint is null))
    {
      throw new ArgumentException("A SQL column cannot contain a null constraint.", nameof(constraints));
    }

    Name = name;
    Type = type;
    Constraints = constraints.ToList();
  }

  internal override IEnumerable<string> Lines()
  {
    var column = $"{D2Writer.ObjectMemberIdentifier(Name)}: {D2Writer.String(Type)}";
    if (Constraints.Count == 0)
    {
      return new[] { column };
    }

    var values = Constraints.Select(constraint => D2Writer.String(constraint.Value));
    var constraintValue = Constraints.Count == 1
      ? values.Single()
      : $"[{string.Join("; ", values)}]";
    return new[] { $"{column} {{ constraint: {constraintValue} }}" };
  }

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());
}

/// <summary>A typed D2 <c>sql_table</c> shape.</summary>
public sealed record D2SqlTable : D2Statement, IEnumerable<D2SqlColumn>
{
  private readonly List<D2SqlColumn> _columns = new();

  /// <summary>The table key.</summary>
  public string Name { get; }
  /// <summary>An optional table label.</summary>
  public string? Label { get; set; }
  /// <summary>Typed styles applied to the table.</summary>
  public D2Style? Style { get; set; }
  /// <summary>An optional click destination.</summary>
  public string? Link { get; set; }
  /// <summary>Optional hover text.</summary>
  public string? Tooltip { get; set; }
  /// <summary>The table's ordered columns.</summary>
  public IReadOnlyList<D2SqlColumn> Columns => _columns;

  /// <summary>Creates an empty SQL table.</summary>
  /// <param name="name">The table key or dotted path.</param>
  /// <param name="label">An optional displayed table label.</param>
  public D2SqlTable(string name, string? label = null)
  {
    _ = D2Writer.Reference(name);
    Name = name;
    Label = label;
  }

  /// <summary>Adds a typed column. Supports collection initializer syntax.</summary>
  /// <param name="column">The column to add.</param>
  public void Add(D2SqlColumn column)
  {
    if (column is null) throw new ArgumentNullException(nameof(column));
    _columns.Add(column);
  }

  internal override IEnumerable<string> Lines()
  {
    var shape = new D2Shape(Name, Label, Shape.SqlTable, Style)
    {
      Link = Link,
      Tooltip = Tooltip,
    };
    foreach (var column in _columns) shape.Add(column);
    return shape.Lines();
  }

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  /// <inheritdoc />
  public IEnumerator<D2SqlColumn> GetEnumerator() => _columns.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
