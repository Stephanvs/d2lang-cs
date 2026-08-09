namespace d2;

public record class D2Connection(
  string First,
  string Second,
  Direction Direction,
  string? Label = ""
)
{
  internal IEnumerable<string> Lines()
  {
    var @base = $"{D2Writer.Reference(First)} {Direction} {D2Writer.Reference(Second)}";
    if (!string.IsNullOrWhiteSpace(Label))
    {
      @base += $": {D2Writer.String(Label)}";
    }

    return new List<string> { @base };
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}
