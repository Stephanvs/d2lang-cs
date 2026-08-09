using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Tests;

[TestClass]
public class UnitTests
{
  [TestMethod]
  public void Diagram_PreservesSimpleReadableOutput()
  {
    var umbrella = new D2Shape("alphabet", "Alphabet Inc", Shape.Rectangle);
    var company = new D2Shape("google", "Google", Shape.Rectangle)
    {
      new D2Shape("gmail", "Gmail", Shape.Rectangle),
      new D2Shape("meet", "Meet", Shape.Rectangle),
      new D2Shape("deepmind", "DeepMind", Shape.Rectangle),
    };

    var connection = new D2Connection(company.Name, umbrella.Name, Direction.To, "BELONGS_TO");

    var diagram = new D2Diagram(new[] { umbrella, company }, new[] { connection });
    var expected = Lines(
      "alphabet: Alphabet Inc {",
      "  shape: rectangle",
      "}",
      "google: Google {",
      "  gmail: Gmail {",
      "    shape: rectangle",
      "  }",
      "  meet: Meet {",
      "    shape: rectangle",
      "  }",
      "  deepmind: DeepMind {",
      "    shape: rectangle",
      "  }",
      "  shape: rectangle",
      "}",
      "google -> alphabet: BELONGS_TO");

    Assert.AreEqual(expected, diagram.ToString());
  }

  [TestMethod]
  public void Shape_QuotesAndEscapesUserControlledSyntax()
  {
    var shape = new D2Shape(
      "service.api",
      "API #1 \"public\" ${secret}\nnext",
      Shape.Rectangle,
      new D2Style("#112233", 2, "color \"blue\" #fff", true, 0.5, 4, false),
      "top # center")
    {
      Icon = "https://example.com/icon \"dark\".svg#v1"
    };

    var expected = Lines(
      "service.api: \"API #1 \\\"public\\\" \\${secret}\\nnext\" {",
      "  icon: \"https://example.com/icon \\\"dark\\\".svg#v1\"",
      "  shape: rectangle",
      "  near: \"top # center\"",
      "  style: {",
      "    stroke: \"#112233\"",
      "    stroke-width: 2",
      "    fill: \"color \\\"blue\\\" #fff\"",
      "    shadow: true",
      "    opacity: 0.5",
      "    stroke-dash: 4",
      "    3d: false",
      "  }",
      "}");

    Assert.AreEqual(expected, shape.ToString());
  }

  [TestMethod]
  public void Shape_EmitsExplicitEmptyLabelAsEmptyString()
  {
    Assert.AreEqual("item: \"\"", new D2Shape("item", string.Empty).ToString());
    Assert.AreEqual("item", new D2Shape("item", null).ToString());
  }

  [TestMethod]
  public void Connection_QuotesEndpointsAndLabel()
  {
    var connection = new D2Connection(
      "source.node",
      "target # node",
      Direction.Both,
      "uses: \"secure\" ${token}");

    Assert.AreEqual(
      "source.node <-> \"target # node\": \"uses: \\\"secure\\\" \\${token}\"",
      connection.ToString());
  }

  [TestMethod]
  public void Style_UsesInvariantNumericFormatting()
  {
    var originalCulture = CultureInfo.CurrentCulture;
    try
    {
      CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
      var style = new D2Style(null, 2, null, null, 0.25, 3, null);

      StringAssert.Contains(style.ToString(), "opacity: 0.25");
      StringAssert.Contains(style.ToString(), "stroke-width: 2");
    }
    finally
    {
      CultureInfo.CurrentCulture = originalCulture;
    }
  }

  [TestMethod]
  [DataRow(-0.01)]
  [DataRow(1.01)]
  [DataRow(double.NaN)]
  [DataRow(double.PositiveInfinity)]
  public void Style_RejectsInvalidOpacity(double opacity)
  {
    var style = new D2Style(null, null, null, null, opacity, null, null);
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => style.ToString());
  }

  [TestMethod]
  [DataRow(0.0, "opacity: 0")]
  [DataRow(1.0, "opacity: 1")]
  public void Style_AcceptsOpacityBoundaries(double opacity, string expected)
  {
    var style = new D2Style(null, null, null, null, opacity, null, null);
    StringAssert.Contains(style.ToString(), expected);
  }

  [TestMethod]
  public void Text_NormalizesAllNewlineConventions()
  {
    var text = new D2Text("label", "one\r\ntwo\rthree\nfour", "md", 1);

    Assert.AreEqual(Lines("label:|md", "one", "two", "three", "four", "|"), text.ToString());
  }

  [TestMethod]
  public void Text_IncreasesDelimiterWhenContentWouldCloseBlock()
  {
    var text = new D2Text("label.text", "one\n|\ntwo", "md", 1);

    Assert.AreEqual(Lines("label.text:||md", "one", "|", "two", "||"), text.ToString());
  }

  [TestMethod]
  public void Shape_QuotesUnsafePathSegmentsWithoutChangingPathSemantics()
  {
    var shape = new D2Shape("system.api#1.endpoint", null);

    Assert.AreEqual("system.\"api#1\".endpoint", shape.ToString());
  }

  [TestMethod]
  public void Text_RejectsInvalidDelimiterAndFormat()
  {
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Text("label", "text", "md", 0).ToString());
    Assert.ThrowsExactly<ArgumentException>(() => new D2Text("label", "text", "md|evil", 1).ToString());
  }

  [TestMethod]
  public void GeneratedDiagram_PassesD2ValidationWhenCliIsAvailable()
  {
    var source = new D2Shape(
      "service.api",
      "API #1 \"public\" ${secret}",
      Shape.Rectangle,
      new D2Style("#112233", 2, "#ffffff", true, 0.5, 4, false))
    {
      Icon = "https://example.com/icon.svg#v1"
    };
    source.Add(new D2Text("tooltip", "first line\r\nsecond # line", "md", 1));
    var target = new D2Shape("target#2", "Target: database", Shape.Cylinder);
    var diagram = new D2Diagram(
      new[] { source, target },
      new[] { new D2Connection(source.Name, target.Name, Direction.To, "calls # safely") });

    var path = Path.Combine(Path.GetTempPath(), $"d2lang-cs-{Guid.NewGuid():N}.d2");
    File.WriteAllText(path, diagram.ToString());

    try
    {
      using var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "d2",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
        }
      };
      process.StartInfo.ArgumentList.Add("validate");
      process.StartInfo.ArgumentList.Add(path);

      try
      {
        process.Start();
      }
      catch (Win32Exception)
      {
        Assert.Inconclusive("The D2 CLI is not installed; parser-backed validation was skipped.");
        return;
      }

      var standardOutput = process.StandardOutput.ReadToEnd();
      var standardError = process.StandardError.ReadToEnd();
      process.WaitForExit();

      Assert.AreEqual(0, process.ExitCode, $"d2 validate failed.{Environment.NewLine}{standardOutput}{standardError}{Environment.NewLine}{diagram}");
    }
    finally
    {
      File.Delete(path);
    }
  }

  private static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines);
}
