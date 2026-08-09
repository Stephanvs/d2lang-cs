namespace d2;

internal static class Utils
{
  public static string StringifyBoolean(bool? value) => D2Writer.Boolean(value is true);

  public static IEnumerable<string> AddLabelAndProperties(
    string name,
    string? label,
    IEnumerable<string> properties)
  {
    return D2Writer.Object(name, label, properties);
  }


  public static IEnumerable<string> Indent(IEnumerable<string> items, int times = 2)
  {
    return D2Writer.Indent(items, times);
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
