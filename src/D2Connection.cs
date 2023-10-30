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
    var @base = $"{First} {Direction} {Second}";
    if (!string.IsNullOrWhiteSpace(Label))
    {
      @base += $": {Label}";
    }

    return new List<string> { @base };
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}