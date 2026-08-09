namespace d2;

/// <summary>Represents the arrowhead direction of a D2 connection.</summary>
/// <param name="Value">The D2 connection operator.</param>
public abstract record Direction(string Value)
{
  /// <summary>A connection pointing from the first endpoint to the second.</summary>
  public static readonly To To = new();
  /// <summary>A connection pointing from the second endpoint to the first.</summary>
  public static readonly From From = new();
  /// <summary>A connection with arrowheads at both endpoints.</summary>
  public static readonly Both Both = new();
  /// <summary>A connection with no arrowhead.</summary>
  public static readonly None None = new();

  /// <summary>Legacy alias for <see cref="To"/>.</summary>
  [Obsolete("Use Direction.To instead.")]
  public static readonly To TO = To;
  /// <summary>Legacy alias for <see cref="From"/>.</summary>
  [Obsolete("Use Direction.From instead.")]
  public static readonly From FROM = From;
  /// <summary>Legacy alias for <see cref="Both"/>.</summary>
  [Obsolete("Use Direction.Both instead.")]
  public static readonly Both BOTH = Both;
  /// <summary>Legacy alias for <see cref="None"/>.</summary>
  [Obsolete("Use Direction.None instead.")]
  public static readonly None NONE = None;
  /// <inheritdoc />
  public sealed override string ToString() => Value;
}

/// <summary>A connection pointing from its first endpoint to its second.</summary>
public sealed record To() : Direction("->");
/// <summary>A connection pointing from its second endpoint to its first.</summary>
public sealed record From() : Direction("<-");
/// <summary>A connection with arrowheads at both endpoints.</summary>
public sealed record Both() : Direction("<->");
/// <summary>A connection with no arrowhead.</summary>
public sealed record None() : Direction("--");
