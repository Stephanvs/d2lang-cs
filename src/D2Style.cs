namespace d2;

public record class D2Style(
  string? Stroke,
  int? StrokeWidth,
  string? Fill,
  bool? Shadow,
  double? Opacity,
  int? StrokeDash,
  bool? ThreeD
)
{
  public IEnumerable<string> Lines()
  {
    if (Opacity is { } opacity &&
      (double.IsNaN(opacity) || double.IsInfinity(opacity) || opacity is < 0 or > 1))
    {
      throw new ArgumentOutOfRangeException(nameof(Opacity), Opacity, "Opacity must be between 0 and 1.");
    }

    var styles = new List<string>();

    if (Stroke is not null) styles.Add($"stroke: {D2Writer.String(Stroke)}");
    if (StrokeWidth is not null) styles.Add($"stroke-width: {D2Writer.Integer(StrokeWidth.Value)}");
    if (Fill is not null) styles.Add($"fill: {D2Writer.String(Fill)}");
    if (Shadow is not null) styles.Add($"shadow: {D2Writer.Boolean(Shadow.Value)}");
    if (Opacity is not null) styles.Add($"opacity: {D2Writer.Number(Opacity.Value)}");
    if (StrokeDash is not null) styles.Add($"stroke-dash: {D2Writer.Integer(StrokeDash.Value)}");
    if (ThreeD is not null) styles.Add($"3d: {D2Writer.Boolean(ThreeD.Value)}");

    return styles.Count == 0
      ? new List<string>()
      : D2Writer.Object("style", null, styles);
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}
