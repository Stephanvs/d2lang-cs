using System.Collections;

namespace d2;

/// <summary>A general D2 shape or container.</summary>
/// <param name="Name">The shape key or dotted path.</param>
/// <param name="Label">An optional displayed label.</param>
/// <param name="Shape">An optional D2 shape kind.</param>
/// <param name="Style">Optional typed styles.</param>
/// <param name="Near">An optional D2 relative placement.</param>
public record class D2Shape(
  string Name,
  string? Label = null,
  Shape? Shape = default,
  D2Style? Style = default,
  string? Near = default
) : D2Statement, IEnumerable<D2Shape>, IEnumerable<D2Connection>, IEnumerable<D2Text>
{
  private readonly List<D2Statement> _statements = new();

  /// <summary>Gets the shape-body statements in serialization order.</summary>
  public IReadOnlyList<D2Statement> Statements => _statements;

  /// <summary>An optional icon URL.</summary>
  public string Icon { get; set; } = string.Empty;

  /// <summary>An optional destination opened when this shape is clicked.</summary>
  public string? Link { get; set; }

  /// <summary>Optional text shown when this shape is hovered.</summary>
  public string? Tooltip { get; set; }

  /// <summary>An optional fixed width for a non-container shape.</summary>
  public int? Width { get; set; }

  /// <summary>An optional fixed height for a non-container shape.</summary>
  public int? Height { get; set; }

  /// <summary>Adds a nested shape. Supports collection initializer syntax.</summary>
  /// <param name="shape">The nested shape to add.</param>
  public void Add(D2Shape shape) => Add((D2Statement)shape);

  /// <summary>Adds a connection to this shape's body.</summary>
  /// <param name="connection">The connection to add.</param>
  public void Add(D2Connection connection) => Add((D2Statement)connection);

  /// <summary>Adds a block-string statement to this shape's body.</summary>
  /// <param name="text">The block-string statement to add.</param>
  public void Add(D2Text text) => Add((D2Statement)text);

  /// <summary>Adds any supported statement to this shape's body.</summary>
  /// <param name="statement">The statement to add.</param>
  public void Add(D2Statement statement)
  {
    if (statement is null)
    {
      throw new ArgumentNullException(nameof(statement));
    }

    _statements.Add(statement);
  }

  internal override IEnumerable<string> Lines()
  {
    var properties = _statements.SelectMany(statement => statement.Lines()).ToList();

    if (!string.IsNullOrWhiteSpace(Icon))
    {
      properties.Add($"icon: {D2Writer.String(Icon)}");
    }

    if (Shape is not null)
    {
      properties.Add($"shape: {Shape}");
    }

    if (Near is { Length: > 0 } near)
    {
      properties.Add($"near: {D2Writer.String(near)}");
    }

    AddPositiveDimension(properties, "width", Width);
    AddPositiveDimension(properties, "height", Height);

    if (Link is not null)
    {
      properties.Add($"link: {D2Writer.String(Link)}");
    }

    if (Tooltip is not null)
    {
      properties.Add($"tooltip: {D2Writer.String(Tooltip)}");
    }

    if (Style is not null)
    {
      properties.AddRange(Style.Lines());
    }

    return D2Writer.Object(Name, Label, properties);
  }

  /// <inheritdoc />
  public override string ToString()
    => string.Join(Environment.NewLine, Lines());

  /// <inheritdoc />
  public IEnumerator<D2Shape> GetEnumerator()
    => _statements.OfType<D2Shape>().GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

  IEnumerator<D2Text> IEnumerable<D2Text>.GetEnumerator()
    => _statements.OfType<D2Text>().GetEnumerator();

  IEnumerator<D2Connection> IEnumerable<D2Connection>.GetEnumerator()
    => _statements.OfType<D2Connection>().GetEnumerator();

  private static void AddPositiveDimension(ICollection<string> properties, string name, int? value)
  {
    if (value is null) return;
    if (value <= 0)
    {
      throw new ArgumentOutOfRangeException(name, value, $"{name} must be greater than zero.");
    }

    properties.Add($"{name}: {D2Writer.Integer(value.Value)}");
  }
}
