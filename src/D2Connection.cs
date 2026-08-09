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

    if (_statements.Count == 0)
    {
      return new[] { @base };
    }

    var openingLine = hasLabel ? $"{@base} {{" : $"{@base}: {{";
    return new[] { openingLine }
      .Concat(D2Writer.Indent(_statements.SelectMany(statement => statement.Lines())))
      .Append("}");
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());

  public IEnumerator<D2Statement> GetEnumerator() => _statements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
