using System.Collections;

namespace d2;

/// <summary>A mutable fluent builder for an ordered <see cref="D2Diagram"/>.</summary>
public sealed class D2DiagramBuilder : IEnumerable<D2Statement>
{
  private readonly List<D2Statement> _statements = new();

  /// <summary>The statements currently in the builder.</summary>
  public IReadOnlyList<D2Statement> Statements => _statements;

  /// <summary>Creates an empty diagram builder.</summary>
  public D2DiagramBuilder()
  {
  }

  /// <summary>Adds a statement. Supports collection initializer syntax.</summary>
  /// <param name="statement">The statement to append.</param>
  public void Add(D2Statement statement)
  {
    if (statement is null) throw new ArgumentNullException(nameof(statement));
    _statements.Add(statement);
  }

  /// <summary>Appends any statement and returns this builder.</summary>
  /// <param name="statement">The statement to append.</param>
  /// <returns>This builder for continued fluent configuration.</returns>
  public D2DiagramBuilder Then(D2Statement statement)
  {
    Add(statement);
    return this;
  }

  /// <summary>Appends a shape and returns this builder.</summary>
  /// <param name="name">The shape key or dotted path.</param>
  /// <param name="label">An optional displayed label.</param>
  /// <param name="shape">An optional D2 shape kind.</param>
  /// <param name="style">Optional typed styles.</param>
  /// <returns>This builder for continued fluent configuration.</returns>
  public D2DiagramBuilder AddShape(
    string name,
    string? label = null,
    Shape? shape = null,
    D2Style? style = null)
    => Then(new D2Shape(name, label, shape, style));

  /// <summary>Appends a connection and returns this builder.</summary>
  /// <param name="first">The first endpoint reference.</param>
  /// <param name="second">The second endpoint reference.</param>
  /// <param name="direction">The connection direction; defaults to <see cref="Direction.To"/>.</param>
  /// <param name="label">An optional displayed label.</param>
  /// <returns>This builder for continued fluent configuration.</returns>
  public D2DiagramBuilder AddConnection(
    string first,
    string second,
    Direction? direction = null,
    string? label = null)
    => Then(new D2Connection(first, second, direction ?? Direction.To, label));

  /// <summary>Creates an immutable snapshot of the current statements.</summary>
  /// <returns>A new diagram containing the builder's current statements.</returns>
  public D2Diagram Build() => new(_statements);

  /// <inheritdoc />
  public IEnumerator<D2Statement> GetEnumerator() => _statements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
