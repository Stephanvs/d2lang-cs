using System.Collections;

namespace d2;

/// <summary>Represents a directed or undirected connection between two D2 objects.</summary>
/// <param name="First">The first endpoint reference.</param>
/// <param name="Second">The second endpoint reference.</param>
/// <param name="Direction">The connection direction.</param>
/// <param name="Label">An optional displayed label.</param>
public record class D2Connection(
  string First,
  string Second,
  Direction Direction,
  string? Label = ""
) : D2Statement, IEnumerable<D2Statement>
{
  private readonly List<D2Statement> _statements = new();

  /// <summary>Gets the additional connection-body statements in serialization order.</summary>
  public IReadOnlyList<D2Statement> Statements => _statements;

  /// <summary>An optional icon URL displayed on this connection.</summary>
  public string? Icon { get; set; }

  /// <summary>An optional destination opened when this connection is clicked.</summary>
  public string? Link { get; set; }

  /// <summary>Optional text shown when this connection is hovered.</summary>
  public string? Tooltip { get; set; }

  /// <summary>Optional typed styles for this connection.</summary>
  public D2Style? Style { get; set; }

  /// <summary>Adds a safely serialized property to the connection body.</summary>
  /// <param name="property">The property to add.</param>
  public void Add(D2Property property) => Add((D2Statement)property);

  /// <summary>Adds a statement to the connection body.</summary>
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
    var @base = $"{D2Writer.Reference(First)} {Direction} {D2Writer.Reference(Second)}";
    var hasLabel = !string.IsNullOrWhiteSpace(Label);
    if (hasLabel)
    {
      @base += $": {D2Writer.String(Label!)}";
    }

    var properties = _statements.SelectMany(statement => statement.Lines()).ToList();
    if (Icon is not null)
    {
      properties.Add($"icon: {D2Writer.String(Icon)}");
    }

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

    if (properties.Count == 0)
    {
      return new[] { @base };
    }

    var openingLine = hasLabel ? $"{@base} {{" : $"{@base}: {{";
    return new[] { openingLine }
      .Concat(D2Writer.Indent(properties))
      .Append("}");
  }

  /// <inheritdoc />
  public override string ToString()
    => string.Join(Environment.NewLine, Lines());

  /// <inheritdoc />
  public IEnumerator<D2Statement> GetEnumerator() => _statements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
