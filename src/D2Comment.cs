namespace d2;

/// <summary>
/// A D2 line comment. Every input line is prefixed so multiline text cannot
/// escape the comment.
/// </summary>
public sealed record D2Comment : D2Statement
{
  /// <summary>Gets the comment text.</summary>
  public string Text { get; }

  /// <summary>Initializes a safely serialized line comment.</summary>
  /// <param name="text">The comment text, which may contain multiple lines.</param>
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

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());
}
