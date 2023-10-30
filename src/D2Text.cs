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
    var sep = "|".Repeat(Pipes);

    return new List<string>()
      .Append($"{Property}:{sep}{Format}")
      .Concat(Text.Split(Environment.NewLine))
      .Append(sep);
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}