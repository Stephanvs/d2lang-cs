namespace d2;

/// <summary>
/// An explicit escape hatch for inserting unescaped D2 source. The caller is
/// responsible for ensuring that the source is valid and trusted.
/// </summary>
public sealed record D2RawStatement : D2Statement
{
  public string Source { get; }

  public D2RawStatement(string source)
  {
    if (source is null)
    {
      throw new ArgumentNullException(nameof(source));
    }

    Source = source;
  }

  internal override IEnumerable<string> Lines() => D2Writer.Lines(Source);

  public override string ToString() => string.Join(Environment.NewLine, Lines());
}
