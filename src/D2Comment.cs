namespace d2;

/// <summary>
/// A D2 line comment. Every input line is prefixed so multiline text cannot
/// escape the comment.
/// </summary>
public sealed record D2Comment : D2Statement
{
  public string Text { get; }

  public D2Comment(string text)
  {
    if (text is null)
    {
      throw new ArgumentNullException(nameof(text));
    }

    Text = text;
  }

  internal override IEnumerable<string> Lines()
    => D2Writer.Lines(Text).Select(line => line.Length == 0 ? "#" : $"# {line}");

  public override string ToString() => string.Join(Environment.NewLine, Lines());
}
