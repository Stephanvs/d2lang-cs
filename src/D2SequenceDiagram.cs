using System.Collections;

namespace d2;

/// <summary>
/// An ordered D2 sequence diagram with fluent helpers for its common statements.
/// Generic <see cref="D2Statement"/> instances can still be added directly.
/// </summary>
public sealed record D2SequenceDiagram : D2Statement, IEnumerable<D2Statement>
{
  private readonly List<D2Statement> _statements = new();

  /// <summary>The sequence diagram key.</summary>
  public string Name { get; }
  /// <summary>An optional displayed label.</summary>
  public string? Label { get; set; }
  /// <summary>Typed styles applied to the sequence diagram.</summary>
  public D2Style? Style { get; set; }
  /// <summary>An optional click destination.</summary>
  public string? Link { get; set; }
  /// <summary>Optional hover text.</summary>
  public string? Tooltip { get; set; }
  /// <summary>The ordered sequence statements.</summary>
  public IReadOnlyList<D2Statement> Statements => _statements;

  /// <summary>Creates an empty sequence diagram.</summary>
  public D2SequenceDiagram(string name, string? label = null)
  {
    _ = D2Writer.Reference(name);
    Name = name;
    Label = label;
  }

  /// <summary>Adds an ordered statement. Supports collection initializer syntax.</summary>
  public void Add(D2Statement statement)
  {
    if (statement is null) throw new ArgumentNullException(nameof(statement));
    _statements.Add(statement);
  }

  /// <summary>Adds an actor or participant and returns this diagram.</summary>
  public D2SequenceDiagram AddParticipant(string name, string? label = null, Shape? shape = null)
  {
    Add(new D2Shape(name, label, shape));
    return this;
  }

  /// <summary>Adds an ordered message and returns this diagram.</summary>
  public D2SequenceDiagram AddMessage(
    string first,
    string second,
    string? label = null,
    Direction? direction = null)
  {
    Add(new D2Connection(first, second, direction ?? Direction.To, label));
    return this;
  }

  /// <summary>Adds a labeled sequence group and returns this diagram.</summary>
  public D2SequenceDiagram AddGroup(string name, params D2Statement[] statements)
  {
    if (statements is null) throw new ArgumentNullException(nameof(statements));
    var group = new D2Shape(name);
    foreach (var statement in statements) group.Add(statement);
    Add(group);
    return this;
  }

  internal override IEnumerable<string> Lines()
  {
    var shape = new D2Shape(Name, Label, Shape.SequenceDiagram, Style)
    {
      Link = Link,
      Tooltip = Tooltip,
    };
    foreach (var statement in _statements) shape.Add(statement);
    return shape.Lines();
  }

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  /// <inheritdoc />
  public IEnumerator<D2Statement> GetEnumerator() => _statements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
