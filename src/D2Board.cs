using System.Collections;

namespace d2;

/// <summary>
/// A named board within a layer, scenario, or step collection.
/// </summary>
public sealed record D2Board : D2Statement, IEnumerable<D2Statement>
{
  private readonly List<D2Statement> _statements;

  /// <summary>Gets the board name.</summary>
  public string Name { get; }

  /// <summary>Gets the statements in their serialization order.</summary>
  public IReadOnlyList<D2Statement> Statements => _statements;

  /// <summary>Initializes an empty board.</summary>
  /// <param name="name">The board name.</param>
  public D2Board(string name)
    : this(name, Array.Empty<D2Statement>())
  {
  }

  /// <summary>Initializes a board with an ordered statement collection.</summary>
  /// <param name="name">The board name.</param>
  /// <param name="statements">The statements to add to the board.</param>
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

  /// <summary>Adds a statement to the end of the board.</summary>
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
    => D2Writer.BlockIdentifier(Name, _statements.SelectMany(statement => statement.Lines()));

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  /// <inheritdoc />
  public IEnumerator<D2Statement> GetEnumerator() => _statements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  private static string ValidateName(string name)
  {
    _ = D2Writer.Identifier(name);
    return name;
  }
}
