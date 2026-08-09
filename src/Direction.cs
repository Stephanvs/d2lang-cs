namespace d2;

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
  public sealed override string ToString() => Value;
}

public sealed record To() : Direction("->");
public sealed record From() : Direction("<-");
public sealed record Both() : Direction("<->");
public sealed record None() : Direction("--");
