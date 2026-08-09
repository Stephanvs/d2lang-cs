namespace d2;

/// <summary>
/// An ordered D2 document. Statement order is retained exactly, which is
/// significant for features such as sequence diagrams and composition boards.
/// </summary>
public record class D2Diagram
{
  private readonly IReadOnlyList<D2Statement> _statements;

  /// <summary>Gets all statements in their serialization order.</summary>
  public IReadOnlyList<D2Statement> Statements => _statements;

  /// <summary>Gets the top-level shape statements.</summary>
  public IEnumerable<D2Shape> Shapes => _statements.OfType<D2Shape>();

  /// <summary>Gets the top-level connection statements.</summary>
  public IEnumerable<D2Connection> Connections => _statements.OfType<D2Connection>();

  /// <summary>Initializes a diagram with shapes followed by connections.</summary>
  /// <param name="Shapes">The shapes to include.</param>
  /// <param name="Connections">The connections to include.</param>
  public D2Diagram(IEnumerable<D2Shape> Shapes, IEnumerable<D2Connection> Connections)
    : this(Combine(Shapes, Connections))
  {
  }

  /// <summary>Initializes a diagram while retaining the supplied statement order.</summary>
  /// <param name="statements">The statements to include.</param>
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

  /// <summary>Returns a new diagram with a shape appended.</summary>
  /// <param name="shape">The shape to append.</param>
  /// <returns>A diagram containing the existing statements and the supplied shape.</returns>
  public D2Diagram Add(D2Shape shape) => Add((D2Statement)shape);

  /// <summary>Returns a new diagram with a connection appended.</summary>
  /// <param name="connection">The connection to append.</param>
  /// <returns>A diagram containing the existing statements and the supplied connection.</returns>
  public D2Diagram Add(D2Connection connection) => Add((D2Statement)connection);

  /// <summary>Returns a new diagram with a statement appended.</summary>
  /// <param name="statement">The statement to append.</param>
  /// <returns>A diagram containing the existing statements and the supplied statement.</returns>
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

  /// <inheritdoc />
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
