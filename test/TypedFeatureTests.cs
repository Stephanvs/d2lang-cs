using System.ComponentModel;
using System.Diagnostics;

namespace Tests;

[TestClass]
public class TypedFeatureTests
{
  [TestMethod]
  public void Style_SerializesTheDocumentedCatalog()
  {
    var style = new D2Style(
      Stroke: "#123456",
      StrokeWidth: 2,
      Fill: "linear-gradient(#fff, #000)",
      Shadow: true,
      Opacity: 0.75,
      StrokeDash: 3,
      ThreeD: false,
      FillPattern: D2FillPattern.Dots,
      BorderRadius: 999,
      Multiple: true,
      DoubleBorder: true,
      Font: D2Font.Mono,
      FontSize: 24,
      FontColor: "#abcdef",
      Animated: true,
      Bold: false,
      Italic: true,
      Underline: true,
      TextTransform: D2TextTransform.Capitalize);

    Assert.AreEqual(
      Lines(
        "style: {",
        "  stroke: \"#123456\"",
        "  stroke-width: 2",
        "  fill: \"linear-gradient(#fff, #000)\"",
        "  shadow: true",
        "  opacity: 0.75",
        "  stroke-dash: 3",
        "  3d: false",
        "  fill-pattern: dots",
        "  border-radius: 999",
        "  multiple: true",
        "  double-border: true",
        "  font: mono",
        "  font-size: 24",
        "  font-color: \"#abcdef\"",
        "  animated: true",
        "  bold: false",
        "  italic: true",
        "  underline: true",
        "  text-transform: capitalize",
        "}"),
      style.ToString());
  }

  [TestMethod]
  public void Style_ValidatesDocumentedRangesAndEnumValues()
  {
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(StrokeWidth: 0).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(StrokeWidth: 16).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(StrokeDash: -1).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(StrokeDash: 11).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(BorderRadius: -1).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(FontSize: 7).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(FontSize: 101).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(FillPattern: (D2FillPattern)100).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(Font: (D2Font)100).ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Style(TextTransform: (D2TextTransform)100).ToString());
  }

  [TestMethod]
  public void ShapeAndConnection_HaveTypedInteractiveAndLayoutProperties()
  {
    var shape = new D2Shape("docs", Shape: Shape.Page, Style: new D2Style(Fill: "#ffffff"))
    {
      Icon = "https://example.com/icon.svg#logo",
      Link = "https://example.com/docs#start",
      Tooltip = "Open: ${safe}",
      Width = 320,
      Height = 180,
    };
    var connection = new D2Connection("client", "docs", Direction.To, "read")
    {
      Icon = "https://example.com/edge.svg#read",
      Link = "https://example.com/edge#read",
      Tooltip = "Read # docs",
      Style = new D2Style(Stroke: "#123456", Animated: true),
    };

    Assert.AreEqual(
      Lines(
        "docs: {",
        "  icon: \"https://example.com/icon.svg#logo\"",
        "  shape: page",
        "  width: 320",
        "  height: 180",
        "  link: \"https://example.com/docs#start\"",
        "  tooltip: \"Open: \\${safe}\"",
        "  style: {",
        "    fill: \"#ffffff\"",
        "  }",
        "}"),
      shape.ToString());
    Assert.AreEqual(
      Lines(
        "client -> docs: read {",
        "  icon: \"https://example.com/edge.svg#read\"",
        "  link: \"https://example.com/edge#read\"",
        "  tooltip: \"Read # docs\"",
        "  style: {",
        "    stroke: \"#123456\"",
        "    animated: true",
        "  }",
        "}"),
      connection.ToString());

    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Shape("bad") { Width = 0 }.ToString());
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Shape("bad") { Height = -1 }.ToString());
  }

  [TestMethod]
  public void SqlTable_SerializesTypedColumnsConstraintsAndReservedNames()
  {
    var table = new D2SqlTable("users", "User records")
    {
      new D2SqlColumn("id", "int", D2SqlConstraint.PrimaryKey, D2SqlConstraint.Unique),
      new D2SqlColumn("account_id", "uuid", D2SqlConstraint.ForeignKey),
      new D2SqlColumn("label", "timestamp with time zone", D2SqlConstraint.Custom("not null")),
    };

    Assert.AreEqual(
      Lines(
        "users: User records {",
        "  id: int { constraint: [primary_key; unique] }",
        "  account_id: uuid { constraint: foreign_key }",
        "  \"label\": timestamp with time zone { constraint: not null }",
        "  shape: sql_table",
        "}"),
      table.ToString());
  }

  [TestMethod]
  public void Class_SerializesFieldsMethodsParametersAndVisibility()
  {
    var @class = new D2Class("parser", "D2 Parser")
    {
      new D2ClassField("reader", "io.RuneReader", D2Visibility.Public),
      new D2ClassField("lookahead", "[]rune", D2Visibility.Private),
      new D2ClassField("label", "string", D2Visibility.Protected),
      new D2ClassMethod(
        "peek",
        "(r rune, eof bool)",
        D2Visibility.Public,
        new D2ClassParameter("count", "uint64")),
      new D2ClassMethod("commit"),
    };

    Assert.AreEqual(
      Lines(
        "parser: D2 Parser {",
        "  \"+reader\": io.RuneReader",
        "  \"-lookahead\": \"[]rune\"",
        "  \"#label\": string",
        "  \"+peek(count uint64)\": \"(r rune, eof bool)\"",
        "  \"commit()\"",
        "  shape: class",
        "}"),
      @class.ToString());
  }

  [TestMethod]
  public void SequenceDiagram_PreservesFluentStatementOrder()
  {
    var sequence = new D2SequenceDiagram("login", "Login")
      .AddParticipant("user", "User", Shape.Person)
      .AddParticipant("api", "API")
      .AddMessage("user", "api", "sign in")
      .AddGroup(
        "retry",
        new D2Connection("api", "api", Direction.To, "refresh"))
      .AddMessage("api", "user", "session", Direction.From);

    Assert.AreEqual(
      Lines(
        "login: Login {",
        "  user: User {",
        "    shape: person",
        "  }",
        "  api: API",
        "  user -> api: sign in",
        "  retry: {",
        "    api -> api: refresh",
        "  }",
        "  api <- user: session",
        "  shape: sequence_diagram",
        "}"),
      sequence.ToString());
  }

  [TestMethod]
  public void Builder_ProvidesFluentAndCollectionInitializerPaths()
  {
    var builder = new D2DiagramBuilder
    {
      new D2Comment("start")
    };
    var diagram = builder
      .AddShape("client", "Client")
      .AddShape("server")
      .AddConnection("client", "server", label: "call")
      .Build();

    Assert.AreEqual(
      Lines("# start", "client: Client", "server", "client -> server: call"),
      diagram.ToString());
  }

  [TestMethod]
  public void PascalCaseDirectionsPreserveLegacyInstances()
  {
#pragma warning disable CS0618
    Assert.AreSame(Direction.To, Direction.TO);
    Assert.AreSame(Direction.From, Direction.FROM);
    Assert.AreSame(Direction.Both, Direction.BOTH);
    Assert.AreSame(Direction.None, Direction.NONE);
#pragma warning restore CS0618
  }

  [TestMethod]
  public void TypedFeatures_PassD2ValidationWhenCliIsAvailable()
  {
    var styled = new D2Shape("styled", Style: new D2Style(
      Stroke: "#123456",
      StrokeWidth: 2,
      Fill: "#ffffff",
      Shadow: true,
      Opacity: 0.8,
      StrokeDash: 2,
      ThreeD: true,
      FillPattern: D2FillPattern.Grain,
      BorderRadius: 12,
      Multiple: true,
      DoubleBorder: true,
      Font: D2Font.Mono,
      FontSize: 20,
      FontColor: "#222222",
      Animated: true,
      Bold: true,
      Italic: false,
      Underline: true,
      TextTransform: D2TextTransform.Uppercase));
    var table = new D2SqlTable("users")
    {
      new D2SqlColumn("id", "int", D2SqlConstraint.PrimaryKey),
      new D2SqlColumn("email", "string", D2SqlConstraint.Unique),
      new D2SqlColumn("label", "timestamp with time zone", D2SqlConstraint.Custom("not null")),
    };
    var @class = new D2Class("service")
    {
      new D2ClassField("repository", "Repository", D2Visibility.Private),
      new D2ClassMethod("find", "User", D2Visibility.Public, new D2ClassParameter("id", "int")),
    };
    var sequence = new D2SequenceDiagram("request")
      .AddParticipant("client")
      .AddParticipant("server")
      .AddMessage("client", "server", "GET # user")
      .AddMessage("server", "client", "User", Direction.From);
    sequence.Link = "https://example.com/docs#sequence";
    sequence.Tooltip = "Request flow";
    var edge = new D2Connection("styled", "users", Direction.To, "opens")
    {
      Icon = "https://example.com/edge.svg#users",
      Link = "https://example.com/docs#users",
      Tooltip = "Open users",
      Style = new D2Style(Stroke: "#ff0000", Animated: true),
    };
    var diagram = new D2Diagram(new D2Statement[] { styled, table, @class, sequence, edge });

    ValidateWithD2(diagram.ToString());
  }

  private static void ValidateWithD2(string source)
  {
    var path = Path.Combine(Path.GetTempPath(), $"d2lang-cs-typed-{Guid.NewGuid():N}.d2");
    File.WriteAllText(path, source);
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

      var output = process.StandardOutput.ReadToEnd();
      var error = process.StandardError.ReadToEnd();
      process.WaitForExit();
      Assert.AreEqual(0, process.ExitCode, $"d2 validate failed.{Environment.NewLine}{output}{error}{Environment.NewLine}{source}");
    }
    finally
    {
      File.Delete(path);
    }
  }

  private static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines);
}
