namespace d2;

public abstract record Direction(string Value)
{
  public readonly static To TO = new();
  public readonly static From FROM = new();
  public readonly static Both BOTH = new();
  public readonly static None NONE = new();
  public sealed override string ToString() => Value;
}

public sealed record To() : Direction("->");
public sealed record From() : Direction("<-");
public sealed record Both() : Direction("<->");
public sealed record None() : Direction("--");
