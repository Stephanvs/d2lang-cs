namespace d2;

public record class D2Style(
  string? Stroke,
  int? StrokeWidth,
  string? Fill,
  bool? Shadow,
  int? Opacity,
  int? StrokeDash,
  bool? ThreeD
)
{
  public IEnumerable<string> Lines()
  {
    var styles = new List<string>();

    if (Stroke is not null) styles.Add($"stroke: {Stroke}");
    if (StrokeWidth is not null) styles.Add($"stroke-width: {StrokeWidth}");
    if (Fill is not null) styles.Add($"fill: {Fill}");
    if (Shadow is not null) styles.Add($"shadow: {Utils.StringifyBoolean(Shadow)}");
    if (Opacity is not null) styles.Add($"opacity: {Opacity}");
    if (StrokeDash is not null) styles.Add($"stroke-dash: {StrokeDash}");
    if (ThreeD is not null) styles.Add($"3d: {Utils.StringifyBoolean(ThreeD)}");

    return styles.Count == 0
      ? new List<string>()
      : Utils.AddLabelAndProperties("style", null, styles);
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}