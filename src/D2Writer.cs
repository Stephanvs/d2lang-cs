using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace d2;

/// <summary>
/// Serializes values into D2 syntax. Keeping these rules in one place prevents
/// individual model types from accidentally emitting user input as D2 code.
/// </summary>
internal static class D2Writer
{
  private static readonly Regex BareIdentifier = new(
    "^[\\p{L}_][\\p{L}\\p{N}_-]*$",
    RegexOptions.CultureInvariant | RegexOptions.Compiled);

  private static readonly Regex BareString = new(
    "^[\\p{L}\\p{N}_][\\p{L}\\p{N} _./-]*$",
    RegexOptions.CultureInvariant | RegexOptions.Compiled);

  internal static string Reference(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("A D2 reference cannot be null, empty, or whitespace.", nameof(value));
    }

    // A dot is meaningful in D2: it navigates to a nested object. Quote each
    // segment independently so path semantics survive without allowing a
    // segment's contents to become syntax.
    var segments = value.Split('.');
    if (segments.Any(string.IsNullOrWhiteSpace))
    {
      throw new ArgumentException("A D2 reference cannot contain an empty path segment.", nameof(value));
    }

    return string.Join(".", segments.Select(IdentifierSegment));
  }

  internal static string Identifier(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("A D2 identifier cannot be null, empty, or whitespace.", nameof(value));
    }

    return IdentifierSegment(value);
  }

  internal static string String(string value)
  {
    if (value is null)
    {
      throw new ArgumentNullException(nameof(value));
    }

    return BareString.IsMatch(value) ? value : Quoted(value);
  }

  internal static string Boolean(bool value) => value ? "true" : "false";

  internal static string Integer(int value) => value.ToString(CultureInfo.InvariantCulture);

  internal static string Number(double value)
  {
    if (double.IsNaN(value) || double.IsInfinity(value))
    {
      throw new ArgumentOutOfRangeException(nameof(value), value, "A D2 number must be finite.");
    }

    return value.ToString("R", CultureInfo.InvariantCulture);
  }

  internal static IEnumerable<string> Block(string name, IEnumerable<string> statements)
    => BlockSerialized(Reference(name), statements);

  internal static IEnumerable<string> BlockIdentifier(string name, IEnumerable<string> statements)
    => BlockSerialized(Identifier(name), statements);

  private static IEnumerable<string> BlockSerialized(string name, IEnumerable<string> statements)
  {
    if (statements is null)
    {
      throw new ArgumentNullException(nameof(statements));
    }

    return new[] { $"{name}: {{" }.Concat(Indent(statements)).Append("}");
  }

  internal static IEnumerable<string> Object(
    string name,
    string? label,
    IEnumerable<string>? properties)
  {
    var propertyLines = properties?.ToList() ?? new List<string>();
    var hasProperties = propertyLines.Count > 0;
    var firstLine = Reference(name);

    if (label is not null || hasProperties)
    {
      firstLine += ":";
    }

    if (label is not null)
    {
      firstLine += $" {String(label)}";
    }

    if (hasProperties)
    {
      firstLine += " {";
      return new[] { firstLine }.Concat(Indent(propertyLines)).Append("}");
    }

    return new[] { firstLine };
  }

  internal static IEnumerable<string> Indent(IEnumerable<string> lines, int spaces = 2)
  {
    if (lines is null)
    {
      throw new ArgumentNullException(nameof(lines));
    }

    if (spaces < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(spaces), spaces, "Indentation cannot be negative.");
    }

    var indentation = new string(' ', spaces);
    return lines.Select(line => indentation + line);
  }

  internal static string[] Lines(string value)
  {
    if (value is null)
    {
      throw new ArgumentNullException(nameof(value));
    }

    return value
      .Replace("\r\n", "\n")
      .Replace('\r', '\n')
      .Split(new[] { '\n' }, StringSplitOptions.None);
  }

  private static string IdentifierSegment(string value)
    => BareIdentifier.IsMatch(value) ? value : Quoted(value);

  private static string Quoted(string value)
  {
    var result = new StringBuilder(value.Length + 2).Append('"');

    foreach (var character in value)
    {
      result.Append(character switch
      {
        '\\' => "\\\\",
        '"' => "\\\"",
        '$' => "\\$",
        '\r' => "\\r",
        '\n' => "\\n",
        '\t' => "\\t",
        _ => character.ToString()
      });
    }

    return result.Append('"').ToString();
  }
}
