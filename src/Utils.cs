namespace d2;

public static class Utils
{
  public static string StringifyBoolean(bool? value) => value switch
  {
    true => "true",
    _ => "false"
  };

  public static IEnumerable<string> AddLabelAndProperties(
    string name,
    string? label,
    IEnumerable<string> properties)
  {
    var hasProperties = properties?.Any() ?? false;

    string firstLine = name ?? string.Empty;
    if (label != null || hasProperties)
    {
      firstLine += ":";
    }

    if (label != null)
    {
      firstLine += $" {label}";
    }

    if (hasProperties)
    {
      firstLine += " {";
    }

    if (properties != null && hasProperties)
    {
      return new List<string> { firstLine }
        .Concat(Indent(properties))
        .Append("}");
    }

    return new[] { firstLine };
  }


  public static IEnumerable<string> Indent(IEnumerable<string> items, int times = 2)
  {
    return items.Select(item => " ".Repeat(times) + item);
  }

  public static string Repeat(this string value, int times)
  {
    var result = string.Empty;
    for (var i = 0; i < times; i++)
    {
      result += value;
    }
    return result;
  }
}