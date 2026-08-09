namespace d2;

/// <summary>Patterns supported by D2's <c>fill-pattern</c> style.</summary>
public enum D2FillPattern
{
  /// <summary>A dotted fill.</summary>
  Dots,
  /// <summary>A lined fill.</summary>
  Lines,
  /// <summary>A grain texture.</summary>
  Grain,
  /// <summary>Disables a fill pattern supplied by a theme.</summary>
  None,
}

/// <summary>Fonts supported by D2's <c>font</c> style.</summary>
public enum D2Font
{
  /// <summary>The D2 monospaced font.</summary>
  Mono,
}

/// <summary>Transforms supported by D2's <c>text-transform</c> style.</summary>
public enum D2TextTransform
{
  /// <summary>Converts text to uppercase.</summary>
  Uppercase,
  /// <summary>Converts text to lowercase.</summary>
  Lowercase,
  /// <summary>Capitalizes words.</summary>
  Capitalize,
  /// <summary>Disables a transform supplied by a theme.</summary>
  None,
}

/// <summary>
/// The documented D2 style catalog. Every setting is optional, so callers can
/// use named arguments to specify only the styles they need.
/// </summary>
/// <param name="Stroke">A CSS color or supported gradient.</param>
/// <param name="StrokeWidth">Stroke width from 1 through 15.</param>
/// <param name="Fill">A CSS color or supported gradient.</param>
/// <param name="Shadow">Whether a shape has a shadow.</param>
/// <param name="Opacity">Opacity from 0 through 1.</param>
/// <param name="StrokeDash">Dash amount from 0 through 10.</param>
/// <param name="ThreeD">Whether a rectangle or square uses the 3D effect.</param>
/// <param name="FillPattern">The shape fill pattern.</param>
/// <param name="BorderRadius">A nonnegative corner radius.</param>
/// <param name="Multiple">Whether a shape uses the multiple-object effect.</param>
/// <param name="DoubleBorder">Whether a supported shape has a double border.</param>
/// <param name="Font">The label font.</param>
/// <param name="FontSize">Font size from 8 through 100.</param>
/// <param name="FontColor">A CSS color or supported gradient.</param>
/// <param name="Animated">Whether a connection or shape is animated.</param>
/// <param name="Bold">Whether label text is bold.</param>
/// <param name="Italic">Whether label text is italic.</param>
/// <param name="Underline">Whether label text is underlined.</param>
/// <param name="TextTransform">The label casing transform.</param>
public record class D2Style(
  string? Stroke = null,
  int? StrokeWidth = null,
  string? Fill = null,
  bool? Shadow = null,
  double? Opacity = null,
  int? StrokeDash = null,
  bool? ThreeD = null,
  D2FillPattern? FillPattern = null,
  int? BorderRadius = null,
  bool? Multiple = null,
  bool? DoubleBorder = null,
  D2Font? Font = null,
  int? FontSize = null,
  string? FontColor = null,
  bool? Animated = null,
  bool? Bold = null,
  bool? Italic = null,
  bool? Underline = null,
  D2TextTransform? TextTransform = null)
{
  /// <summary>Serializes this style as a D2 <c>style</c> block.</summary>
  public IEnumerable<string> Lines()
  {
    ValidateRange(StrokeWidth, 1, 15, nameof(StrokeWidth));
    ValidateRange(StrokeDash, 0, 10, nameof(StrokeDash));
    ValidateMinimum(BorderRadius, 0, nameof(BorderRadius));
    ValidateRange(FontSize, 8, 100, nameof(FontSize));
    if (Opacity is { } opacity &&
      (double.IsNaN(opacity) || double.IsInfinity(opacity) || opacity is < 0 or > 1))
    {
      throw new ArgumentOutOfRangeException(nameof(Opacity), Opacity, "Opacity must be between 0 and 1.");
    }

    var styles = new List<string>();

    AddString(styles, "stroke", Stroke);
    AddInteger(styles, "stroke-width", StrokeWidth);
    AddString(styles, "fill", Fill);
    AddBoolean(styles, "shadow", Shadow);
    AddNumber(styles, "opacity", Opacity);
    AddInteger(styles, "stroke-dash", StrokeDash);
    AddBoolean(styles, "3d", ThreeD);
    AddEnum(styles, "fill-pattern", FillPattern, FillPatternValue);
    AddInteger(styles, "border-radius", BorderRadius);
    AddBoolean(styles, "multiple", Multiple);
    AddBoolean(styles, "double-border", DoubleBorder);
    AddEnum(styles, "font", Font, FontValue);
    AddInteger(styles, "font-size", FontSize);
    AddString(styles, "font-color", FontColor);
    AddBoolean(styles, "animated", Animated);
    AddBoolean(styles, "bold", Bold);
    AddBoolean(styles, "italic", Italic);
    AddBoolean(styles, "underline", Underline);
    AddEnum(styles, "text-transform", TextTransform, TextTransformValue);

    return styles.Count == 0
      ? Array.Empty<string>()
      : D2Writer.Object("style", null, styles);
  }

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  private static void AddString(ICollection<string> styles, string name, string? value)
  {
    if (value is not null) styles.Add($"{name}: {D2Writer.String(value)}");
  }

  private static void AddInteger(ICollection<string> styles, string name, int? value)
  {
    if (value is not null) styles.Add($"{name}: {D2Writer.Integer(value.Value)}");
  }

  private static void AddNumber(ICollection<string> styles, string name, double? value)
  {
    if (value is not null) styles.Add($"{name}: {D2Writer.Number(value.Value)}");
  }

  private static void AddBoolean(ICollection<string> styles, string name, bool? value)
  {
    if (value is not null) styles.Add($"{name}: {D2Writer.Boolean(value.Value)}");
  }

  private static void AddEnum<T>(
    ICollection<string> styles,
    string name,
    T? value,
    Func<T, string> serialize)
    where T : struct
  {
    if (value is not null) styles.Add($"{name}: {serialize(value.Value)}");
  }

  private static void ValidateRange(int? value, int minimum, int maximum, string name)
  {
    if (value is { } actual && (actual < minimum || actual > maximum))
    {
      throw new ArgumentOutOfRangeException(name, actual, $"{name} must be between {minimum} and {maximum}.");
    }
  }

  private static void ValidateMinimum(int? value, int minimum, string name)
  {
    if (value is { } actual && actual < minimum)
    {
      throw new ArgumentOutOfRangeException(name, actual, $"{name} must be at least {minimum}.");
    }
  }

  private static string FillPatternValue(D2FillPattern value) => value switch
  {
    D2FillPattern.Dots => "dots",
    D2FillPattern.Lines => "lines",
    D2FillPattern.Grain => "grain",
    D2FillPattern.None => "none",
    _ => throw UnknownEnum(nameof(FillPattern), value),
  };

  private static string FontValue(D2Font value) => value switch
  {
    D2Font.Mono => "mono",
    _ => throw UnknownEnum(nameof(Font), value),
  };

  private static string TextTransformValue(D2TextTransform value) => value switch
  {
    D2TextTransform.Uppercase => "uppercase",
    D2TextTransform.Lowercase => "lowercase",
    D2TextTransform.Capitalize => "capitalize",
    D2TextTransform.None => "none",
    _ => throw UnknownEnum(nameof(TextTransform), value),
  };

  private static ArgumentOutOfRangeException UnknownEnum<T>(string name, T value)
    => new(name, value, $"Unknown {name} value.");
}
