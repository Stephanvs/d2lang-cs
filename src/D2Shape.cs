using System.Collections;

namespace d2;

public record class D2Shape(
  string Name,
  string? Label,
  Shape? Shape = default,
  D2Style? Style = default,
  string? Near = default
) : D2Statement, IEnumerable<D2Shape>, IEnumerable<D2Connection>, IEnumerable<D2Text>
{
  private readonly List<D2Statement> _statements = new();

  public IReadOnlyList<D2Statement> Statements => _statements;

  public string Icon { get; set; } = string.Empty;

  public void Add(D2Shape shape) => Add((D2Statement)shape);

  public void Add(D2Connection connection) => Add((D2Statement)connection);

  public void Add(D2Text text) => Add((D2Statement)text);

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

    if (Style is not null)
    {
      properties.AddRange(Style.Lines());
    }

    return D2Writer.Object(Name, Label, properties);
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());

  public IEnumerator<D2Shape> GetEnumerator()
    => _statements.OfType<D2Shape>().GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

  IEnumerator<D2Text> IEnumerable<D2Text>.GetEnumerator()
    => _statements.OfType<D2Text>().GetEnumerator();

  IEnumerator<D2Connection> IEnumerable<D2Connection>.GetEnumerator()
    => _statements.OfType<D2Connection>().GetEnumerator();
}
