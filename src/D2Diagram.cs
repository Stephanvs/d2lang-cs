namespace d2;

/// <summary>
/// An ordered D2 document. Statement order is retained exactly, which is
/// significant for features such as sequence diagrams and composition boards.
/// </summary>
public record class D2Diagram
{
  private readonly IReadOnlyList<D2Statement> _statements;

  public IReadOnlyList<D2Statement> Statements => _statements;

  public IEnumerable<D2Shape> Shapes => _statements.OfType<D2Shape>();

  public IEnumerable<D2Connection> Connections => _statements.OfType<D2Connection>();

  public D2Diagram(IEnumerable<D2Shape> Shapes, IEnumerable<D2Connection> Connections)
    : this(Combine(Shapes, Connections))
  {
  }

  public D2Diagram(IEnumerable<D2Statement> statements)
  {
    if (statements is null)
    {
      throw new ArgumentNullException(nameof(statements));
    }

    var materialized = statements.ToList();
    if (materialized.Any(statement => statement is null))
    {
      throw new ArgumentException("A diagram cannot contain a null statement.", nameof(statements));
    }

    _statements = materialized;
  }

  public D2Diagram Add(D2Shape shape) => Add((D2Statement)shape);

  public D2Diagram Add(D2Connection connection) => Add((D2Statement)connection);

  public D2Diagram Add(D2Statement statement)
  {
    if (statement is null)
    {
      throw new ArgumentNullException(nameof(statement));
    }
    return new D2Diagram(_statements.Append(statement));
  }

  internal IEnumerable<string> Lines()
    => _statements.SelectMany(statement => statement.Lines());

  public override string ToString() => string.Join(Environment.NewLine, Lines());

  private static IEnumerable<D2Statement> Combine(
    IEnumerable<D2Shape> shapes,
    IEnumerable<D2Connection> connections)
  {
    if (shapes is null)
    {
      throw new ArgumentNullException(nameof(shapes));
    }

    if (connections is null)
    {
      throw new ArgumentNullException(nameof(connections));
    }

    return shapes.Cast<D2Statement>().Concat(connections);
  }
}
