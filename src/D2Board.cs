using System.Collections;

namespace d2;

/// <summary>
/// A named board within a layer, scenario, or step collection.
/// </summary>
public sealed record D2Board : D2Statement, IEnumerable<D2Statement>
{
  private readonly List<D2Statement> _statements;

  public string Name { get; }

  public IReadOnlyList<D2Statement> Statements => _statements;

  public D2Board(string name)
    : this(name, Array.Empty<D2Statement>())
  {
  }

  public D2Board(string name, IEnumerable<D2Statement> statements)
  {
    Name = ValidateName(name);
    if (statements is null)
    {
      throw new ArgumentNullException(nameof(statements));
    }

    _statements = statements.ToList();
    if (_statements.Any(statement => statement is null))
    {
      throw new ArgumentException("A board cannot contain a null statement.", nameof(statements));
    }
  }

  public void Add(D2Statement statement)
  {
    if (statement is null)
    {
      throw new ArgumentNullException(nameof(statement));
    }

    _statements.Add(statement);
  }

  internal override IEnumerable<string> Lines()
    => D2Writer.BlockIdentifier(Name, _statements.SelectMany(statement => statement.Lines()));

  public override string ToString() => string.Join(Environment.NewLine, Lines());

  public IEnumerator<D2Statement> GetEnumerator() => _statements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  private static string ValidateName(string name)
  {
    _ = D2Writer.Identifier(name);
    return name;
  }
}
