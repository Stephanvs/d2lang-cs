namespace d2;

/// <summary>
/// A statement that can be written in a D2 document or object body.
/// </summary>
public abstract record D2Statement
{
  internal abstract IEnumerable<string> Lines();
}
