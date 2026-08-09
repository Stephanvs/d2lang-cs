using System.Collections;

namespace d2;

/// <summary>UML visibility prefixes supported by D2 class members.</summary>
public enum D2Visibility
{
  /// <summary>No explicit prefix; D2 treats this as public.</summary>
  Default,
  /// <summary>Explicit public visibility (<c>+</c>).</summary>
  Public,
  /// <summary>Private visibility (<c>-</c>).</summary>
  Private,
  /// <summary>Protected visibility (<c>#</c>).</summary>
  Protected,
}

/// <summary>A typed parameter displayed in a UML class method signature.</summary>
public sealed record D2ClassParameter
{
  /// <summary>The parameter name.</summary>
  public string Name { get; }
  /// <summary>The parameter type.</summary>
  public string Type { get; }

  /// <summary>Creates a class method parameter.</summary>
  /// <param name="name">The parameter name.</param>
  /// <param name="type">The parameter type displayed by D2.</param>
  public D2ClassParameter(string name, string type)
  {
    _ = D2Writer.Identifier(name);
    if (string.IsNullOrWhiteSpace(type))
    {
      throw new ArgumentException("A class parameter type cannot be null, empty, or whitespace.", nameof(type));
    }

    Name = name;
    Type = type;
  }

  internal string Display() => $"{Name} {Type}";
}

/// <summary>A field or method in a typed <see cref="D2Class"/>.</summary>
public abstract class D2ClassMember
{
  /// <summary>The UML visibility of the member.</summary>
  public D2Visibility Visibility { get; }

  private protected D2ClassMember(D2Visibility visibility)
  {
    if (!Enum.IsDefined(typeof(D2Visibility), visibility))
    {
      throw new ArgumentOutOfRangeException(nameof(visibility), visibility, "Unknown UML visibility.");
    }
    Visibility = visibility;
  }

  private protected abstract string Signature { get; }

  private protected abstract string? Result { get; }

  internal IEnumerable<string> Lines()
  {
    var displayKey = VisibilityPrefix(Visibility) + Signature;
    var key = Visibility == D2Visibility.Default && this is D2ClassField
      ? D2Writer.ObjectMemberIdentifier(displayKey)
      : D2Writer.Identifier(displayKey);
    return Result is null
      ? new[] { key }
      : new[] { $"{key}: {D2Writer.String(Result)}" };
  }

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  private static string VisibilityPrefix(D2Visibility visibility) => visibility switch
  {
    D2Visibility.Default => string.Empty,
    D2Visibility.Public => "+",
    D2Visibility.Private => "-",
    D2Visibility.Protected => "#",
    _ => throw new ArgumentOutOfRangeException(nameof(visibility), visibility, "Unknown UML visibility."),
  };
}

/// <summary>A typed field in a D2 UML class.</summary>
public sealed class D2ClassField : D2ClassMember
{
  /// <summary>The field name.</summary>
  public string Name { get; }
  /// <summary>The optional field type.</summary>
  public string? Type { get; }

  /// <summary>Creates a class field.</summary>
  /// <param name="name">The field name.</param>
  /// <param name="type">The optional field type.</param>
  /// <param name="visibility">The UML visibility prefix.</param>
  public D2ClassField(string name, string? type = null, D2Visibility visibility = D2Visibility.Default)
    : base(visibility)
  {
    _ = D2Writer.Identifier(name);
    Name = name;
    Type = type;
  }

  private protected override string Signature => Name;
  private protected override string? Result => Type;
}

/// <summary>A typed method in a D2 UML class.</summary>
public sealed class D2ClassMethod : D2ClassMember
{
  /// <summary>The method name.</summary>
  public string Name { get; }
  /// <summary>The optional return type; <see langword="null"/> means void.</summary>
  public string? ReturnType { get; }
  /// <summary>The method parameters.</summary>
  public IReadOnlyList<D2ClassParameter> Parameters { get; }

  /// <summary>Creates a class method.</summary>
  /// <param name="name">The method name.</param>
  /// <param name="returnType">The optional return type; <see langword="null"/> represents void.</param>
  /// <param name="visibility">The UML visibility prefix.</param>
  /// <param name="parameters">The method parameters in display order.</param>
  public D2ClassMethod(
    string name,
    string? returnType = null,
    D2Visibility visibility = D2Visibility.Default,
    params D2ClassParameter[] parameters)
    : base(visibility)
  {
    _ = D2Writer.Identifier(name);
    if (parameters is null) throw new ArgumentNullException(nameof(parameters));
    if (parameters.Any(parameter => parameter is null))
    {
      throw new ArgumentException("A class method cannot contain a null parameter.", nameof(parameters));
    }

    Name = name;
    ReturnType = returnType;
    Parameters = parameters.ToList();
  }

  private protected override string Signature
    => $"{Name}({string.Join(", ", Parameters.Select(parameter => parameter.Display()))})";

  private protected override string? Result => ReturnType;
}

/// <summary>A typed D2 UML <c>class</c> shape.</summary>
public sealed record D2Class : D2Statement, IEnumerable<D2ClassMember>
{
  private readonly List<D2ClassMember> _members = new();

  /// <summary>The class key.</summary>
  public string Name { get; }
  /// <summary>An optional displayed class label.</summary>
  public string? Label { get; set; }
  /// <summary>Typed styles applied to the class.</summary>
  public D2Style? Style { get; set; }
  /// <summary>An optional click destination.</summary>
  public string? Link { get; set; }
  /// <summary>Optional hover text.</summary>
  public string? Tooltip { get; set; }
  /// <summary>The class's ordered members.</summary>
  public IReadOnlyList<D2ClassMember> Members => _members;

  /// <summary>Creates an empty UML class.</summary>
  /// <param name="name">The class key or dotted path.</param>
  /// <param name="label">An optional displayed class label.</param>
  public D2Class(string name, string? label = null)
  {
    _ = D2Writer.Reference(name);
    Name = name;
    Label = label;
  }

  /// <summary>Adds a typed member. Supports collection initializer syntax.</summary>
  /// <param name="member">The field or method to add.</param>
  public void Add(D2ClassMember member)
  {
    if (member is null) throw new ArgumentNullException(nameof(member));
    _members.Add(member);
  }

  internal override IEnumerable<string> Lines()
  {
    var shape = new D2Shape(Name, Label, Shape.Class, Style)
    {
      Link = Link,
      Tooltip = Tooltip,
    };
    foreach (var member in _members) shape.Add(new MemberStatement(member));
    return shape.Lines();
  }

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  /// <inheritdoc />
  public IEnumerator<D2ClassMember> GetEnumerator() => _members.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  private sealed record MemberStatement(D2ClassMember Member) : D2Statement
  {
    internal override IEnumerable<string> Lines() => Member.Lines();
  }
}
