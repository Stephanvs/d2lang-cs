using System.ComponentModel;
using System.Diagnostics;

namespace Tests;

[TestClass]
public class ExtensibleModelTests
{
  [TestMethod]
  public void Diagram_PreservesMixedStatementOrder()
  {
    var diagram = new D2Diagram(new D2Statement[]
    {
      new D2Property("direction", "right"),
      new D2Shape("actor", "Actor"),
      new D2Comment("the request happens next"),
      new D2Connection("actor", "service", Direction.To, "request"),
      new D2Shape("service", "Service"),
    });

    Assert.AreEqual(
      Lines(
        "direction: right",
        "actor: Actor",
        "# the request happens next",
        "actor -> service: request",
        "service: Service"),
      diagram.ToString());
  }

  [TestMethod]
  public void Shape_PreservesMixedBodyStatementOrder()
  {
    var sequence = new D2Shape("login", "Login", Shape.SequenceDiagram)
    {
      new D2Shape("user", "User"),
      new D2Shape("api", "API"),
      new D2Connection("user", "api", Direction.To, "sign in"),
      new D2Comment("the response must remain after the request"),
      new D2Connection("api", "user", Direction.To, "session"),
    };

    Assert.AreEqual(
      Lines(
        "login: Login {",
        "  user: User",
        "  api: API",
        "  user -> api: sign in",
        "  # the response must remain after the request",
        "  api -> user: session",
        "  shape: sequence_diagram",
        "}"),
      sequence.ToString());
  }

  [TestMethod]
  public void Comment_PrefixesEveryLineAndRawStatementIsExplicitlyUnescaped()
  {
    var diagram = new D2Diagram(new D2Statement[]
    {
      new D2Comment("safe\r\nx -> injected\rempty next\n"),
      new D2RawStatement("raw -> syntax\r\nraw.style.opacity: 0.5"),
    });

    Assert.AreEqual(
      Lines(
        "# safe",
        "# x -> injected",
        "# empty next",
        "#",
        "raw -> syntax",
        "raw.style.opacity: 0.5"),
      diagram.ToString());
  }

  [TestMethod]
  public void RootProperties_SafelySerializeNestedConfiguration()
  {
    var config = new D2Property("vars", new D2Statement[]
    {
      new D2Property("d2-config", new D2Statement[]
      {
        new D2Property("theme-id", 300),
        new D2Property("center", true),
        new D2Property("layout-engine", "elk"),
      }),
    });
    var diagram = new D2Diagram(new D2Statement[]
    {
      new D2Property("direction", "right"),
      config,
      new D2Property("style.fill", "#f4a261"),
    });

    Assert.AreEqual(
      Lines(
        "direction: right",
        "vars: {",
        "  d2-config: {",
        "    theme-id: 300",
        "    center: true",
        "    layout-engine: elk",
        "  }",
        "}",
        "style.fill: \"#f4a261\""),
      diagram.ToString());
  }

  [TestMethod]
  public void Connection_SupportsOrderedGenericProperties()
  {
    var connection = new D2Connection("client", "server", Direction.To, "call")
    {
      new D2Property("style", new D2Statement[]
      {
        new D2Property("stroke", "#112233"),
        new D2Property("opacity", 0.5),
      }),
      new D2Property("link", "https://example.com/path#details"),
      new D2Property("tooltip", "uses: ${token}"),
    };

    Assert.AreEqual(
      Lines(
        "client -> server: call {",
        "  style: {",
        "    stroke: \"#112233\"",
        "    opacity: 0.5",
        "  }",
        "  link: \"https://example.com/path#details\"",
        "  tooltip: \"uses: \\${token}\"",
        "}"),
      connection.ToString());
  }

  [TestMethod]
  public void Boards_SupportNestedLayersScenariosAndSteps()
  {
    var steps = new D2BoardCollection(D2BoardKind.Steps)
    {
      new D2Board("1") { new D2Shape("queued", null) },
      new D2Board("2") { new D2Connection("queued", "done", Direction.To) },
    };
    var scenarios = new D2BoardCollection(D2BoardKind.Scenarios)
    {
      new D2Board("happy.path")
      {
        new D2Shape("worker", null),
        steps,
      },
    };
    var layers = new D2BoardCollection(D2BoardKind.Layers)
    {
      new D2Board("detail")
      {
        new D2Property("direction", "right"),
        new D2Connection("client", "worker", Direction.To),
        scenarios,
      },
    };
    var diagram = new D2Diagram(new D2Statement[]
    {
      new D2Shape("overview", null),
      layers,
    });

    Assert.AreEqual(
      Lines(
        "overview",
        "layers: {",
        "  detail: {",
        "    direction: right",
        "    client -> worker",
        "    scenarios: {",
        "      \"happy.path\": {",
        "        worker",
        "        steps: {",
        "          \"1\": {",
        "            queued",
        "          }",
        "          \"2\": {",
        "            queued -> done",
        "          }",
        "        }",
        "      }",
        "    }",
        "  }",
        "}"),
      diagram.ToString());
  }

  [TestMethod]
  public void GenericNamesAreValidatedAndNumbersMustBeFinite()
  {
    Assert.ThrowsExactly<ArgumentException>(() => new D2Property(" ", "value"));
    Assert.ThrowsExactly<ArgumentException>(() => new D2Property("style..fill", "value"));
    Assert.ThrowsExactly<ArgumentException>(() => new D2Board(string.Empty));
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new D2Property("opacity", double.NaN).ToString());
  }

  [TestMethod]
  public void ExtensibleDocument_PassesD2ValidationWhenCliIsAvailable()
  {
    var connection = new D2Connection("client", "server", Direction.To, "calls # safely")
    {
      new D2Property("style", new D2Statement[]
      {
        new D2Property("stroke", "#112233"),
        new D2Property("opacity", 0.5),
      }),
      new D2Property("link", "https://example.com/docs#api"),
      new D2Property("tooltip", "API: ${not-code}"),
    };
    var layers = new D2BoardCollection(D2BoardKind.Layers)
    {
      new D2Board("detail")
      {
        new D2Shape("database", "Primary #1", Shape.Cylinder),
        new D2BoardCollection(D2BoardKind.Scenarios)
        {
          new D2Board("failover")
          {
            new D2Connection("database", "replica", Direction.To, "replicates"),
          },
        },
      },
    };
    var diagram = new D2Diagram(new D2Statement[]
    {
      new D2Comment("generated through ordered statements"),
      new D2Property("direction", "right"),
      new D2Property("vars", new D2Statement[]
      {
        new D2Property("d2-config", new D2Statement[]
        {
          new D2Property("pad", 0),
          new D2Property("center", true),
        }),
      }),
      new D2Shape("client", "Client"),
      new D2Shape("server", "Server"),
      connection,
      layers,
    });

    ValidateWithD2(diagram.ToString());
  }

  private static void ValidateWithD2(string source)
  {
    var path = Path.Combine(Path.GetTempPath(), $"d2lang-cs-model-{Guid.NewGuid():N}.d2");
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

      var standardOutput = process.StandardOutput.ReadToEnd();
      var standardError = process.StandardError.ReadToEnd();
      process.WaitForExit();

      Assert.AreEqual(
        0,
        process.ExitCode,
        $"d2 validate failed.{Environment.NewLine}{standardOutput}{standardError}{Environment.NewLine}{source}");
    }
    finally
    {
      File.Delete(path);
    }
  }

  private static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines);
}
