using System.Collections;

namespace d2;

public record class D2Connection(
  string First,
  string Second,
  Direction Direction,
  string? Label = ""
) : D2Statement, IEnumerable<D2Statement>
{
  private readonly List<D2Statement> _statements = new();

  public IReadOnlyList<D2Statement> Statements => _statements;

  /// <summary>An optional icon URL displayed on this connection.</summary>
  public string? Icon { get; set; }

  /// <summary>An optional destination opened when this connection is clicked.</summary>
  public string? Link { get; set; }

  /// <summary>Optional text shown when this connection is hovered.</summary>
  public string? Tooltip { get; set; }

  /// <summary>Optional typed styles for this connection.</summary>
  public D2Style? Style { get; set; }

  public void Add(D2Property property) => Add((D2Statement)property);

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

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());

  public IEnumerator<D2Statement> GetEnumerator() => _statements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
