namespace d2;

public record class D2Shape(
  string Name,
  string? Label,
  Shape? Shape = default,
  D2Style? Style = default,
  string? Near = default
)
{
  private readonly List<D2Shape> _shapes = new();
  private readonly List<D2Connection> _connections = new();
  private readonly List<D2Text> _texts = new();

  public string Icon { get; set; } = string.Empty;

  public void Add(D2Shape shape) => _shapes.Add(shape);

  public void Add(D2Connection connection) => _connections.Add(connection);

  public void Add(D2Text text) => _texts.Add(text);

  internal IEnumerable<string> Lines()
  {
    var shapes = _shapes.SelectMany(s => s.Lines()).ToList();
    var connections = _connections.SelectMany(c => c.Lines()).ToList();
    var texts = _texts.SelectMany(t => t.Lines()).ToList();

    var properties = shapes.Concat(connections).Concat(texts).ToList();

    if (!string.IsNullOrWhiteSpace(Icon))
    {
      properties.Add($"icon: {Icon}");
    }

    if (Shape is not null)
    {
      properties.Add($"shape: {Shape}");
    }

    if (!string.IsNullOrEmpty(Near))
    {
      properties.Add($"near: {Near}");
    }

    if (Style is not null)
    {
      properties.AddRange(Style.Lines());
    }

    return Utils.AddLabelAndProperties(Name, Label, properties);
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}