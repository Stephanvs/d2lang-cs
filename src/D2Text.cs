namespace d2;

public record D2Text(
  string Property,
  string Text,
  string Format,
  int Pipes
)
{
  internal IEnumerable<string> Lines()
  {
    if (Pipes < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(Pipes), Pipes, "A block string delimiter needs at least one pipe.");
    }

    if (string.IsNullOrWhiteSpace(Format) || !Format.All(IsAsciiFormatCharacter))
    {
      throw new ArgumentException("The block string format must contain only ASCII letters, digits, underscores, or hyphens.", nameof(Format));
    }

    var textLines = D2Writer.Lines(Text);
    var pipeCount = Pipes;
    while (textLines.Contains(new string('|', pipeCount), StringComparer.Ordinal))
    {
      pipeCount++;
    }

    var separator = new string('|', pipeCount);

    return new[] { $"{D2Writer.Reference(Property)}:{separator}{Format}" }
      .Concat(textLines)
      .Append(separator);
  }

  private static bool IsAsciiFormatCharacter(char value)
    => value is >= 'a' and <= 'z'
      or >= 'A' and <= 'Z'
      or >= '0' and <= '9'
      or '_' or '-';

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}
