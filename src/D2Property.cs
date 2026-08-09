namespace d2;

/// <summary>
/// A safely serialized D2 property. Values are always treated as data, never as
/// D2 source code. Use <see cref="D2RawStatement"/> when raw D2 is intentional.
/// </summary>
public sealed record D2Property : D2Statement
{
  private readonly IReadOnlyList<D2Statement>? _statements;

  /// <summary>Gets the property name or dotted property path.</summary>
  public string Name { get; }

  /// <summary>Gets the scalar property value, or <see langword="null"/> for a block property.</summary>
  public object? Value { get; }

  /// <summary>Gets whether this property contains a statement block.</summary>
  public bool IsBlock => _statements is not null;

  /// <summary>Gets the block statements, or an empty list for a scalar property.</summary>
  public IReadOnlyList<D2Statement> Statements => _statements ?? Array.Empty<D2Statement>();

  /// <summary>Initializes a string-valued property.</summary>
  /// <param name="name">The property name or dotted path.</param>
  /// <param name="value">The string value.</param>
  public D2Property(string name, string value)
  {
    Name = ValidateName(name);
    if (value is null)
    {
      throw new ArgumentNullException(nameof(value));
    }
    Value = value;
  }

  /// <summary>Initializes a Boolean-valued property.</summary>
  /// <param name="name">The property name or dotted path.</param>
  /// <param name="value">The Boolean value.</param>
  public D2Property(string name, bool value)
  {
    Name = ValidateName(name);
    Value = value;
  }

  /// <summary>Initializes an integer-valued property.</summary>
  /// <param name="name">The property name or dotted path.</param>
  /// <param name="value">The integer value.</param>
  public D2Property(string name, int value)
  {
    Name = ValidateName(name);
    Value = value;
  }

  /// <summary>Initializes a numeric property serialized with invariant formatting.</summary>
  /// <param name="name">The property name or dotted path.</param>
  /// <param name="value">The finite floating-point value.</param>
  public D2Property(string name, double value)
  {
    Name = ValidateName(name);
    Value = value;
  }

  /// <summary>Initializes a block property containing ordered statements.</summary>
  /// <param name="name">The property name or dotted path.</param>
  /// <param name="statements">The statements in the property block.</param>
  public D2Property(string name, IEnumerable<D2Statement> statements)
  {
    Name = ValidateName(name);
    _statements = Materialize(statements, nameof(statements));
  }

  internal override IEnumerable<string> Lines()
  {
    if (_statements is null)
    {
      return new[] { $"{D2Writer.Reference(Name)}: {SerializeValue()}" };
    }

    return D2Writer.Block(Name, _statements.SelectMany(statement => statement.Lines()));
  }

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  private string SerializeValue() => Value switch
  {
    string value => D2Writer.String(value),
    bool value => D2Writer.Boolean(value),
    int value => D2Writer.Integer(value),
    double value => D2Writer.Number(value),
    _ => throw new InvalidOperationException("A scalar D2 property must have a supported value."),
  };

  private static string ValidateName(string name)
  {
    _ = D2Writer.Reference(name);
    return name;
  }

  private static IReadOnlyList<D2Statement> Materialize(
    IEnumerable<D2Statement> statements,
    string parameterName)
  {
    if (statements is null)
    {
      throw new ArgumentNullException(parameterName);
    }

    var result = statements.ToList();
    if (result.Any(statement => statement is null))
    {
      throw new ArgumentException("A D2 statement collection cannot contain null.", parameterName);
    }

    return result;
  }
}
