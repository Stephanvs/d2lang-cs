# d2lang-cs

![d2lang-cs banner](docs/assets/img/banner.png)

`d2lang-cs` is an unofficial .NET library for building [D2](https://github.com/terrastruct/d2) source from strongly typed C# objects. It produces `.d2` text; it does not bundle the D2 renderer or invoke the D2 CLI.

Use the library anywhere you need to construct, store, inspect, or send D2 source. Install the separate D2 CLI only when your application also needs to validate or render that source as SVG, PNG, or another output format.

## Install

```bash
dotnet add package d2lang-cs
```

The package provides `netstandard2.0`, `net8.0`, and `net10.0` assets. Applications can therefore use the broad .NET Standard library surface or target the current supported .NET LTS releases directly.

## Quick start

```csharp
using d2;

var diagram = new D2DiagramBuilder()
    .AddShape("client", "Client", Shape.Person)
    .AddShape(
        "api",
        "API",
        Shape.Rectangle,
        new D2Style(Fill: "#f4a261", BorderRadius: 8))
    .AddConnection("client", "api", label: "GET /users")
    .Build();

var source = diagram.ToString();
Console.WriteLine(source);
```

This produces:

```d2
client: Client {
  shape: person
}
api: API {
  shape: rectangle
  style: {
    fill: "#f4a261"
    border-radius: 8
  }
}
client -> api: GET /users
```

To render it, write `source` to a file and use an independently installed D2 CLI:

```bash
d2 diagram.d2 diagram.svg
```

## Core model

A `D2Diagram` is an ordered collection of `D2Statement` values. Shapes, connections, comments, properties, composition boards, typed special shapes, and explicit raw statements all participate in that model. Statement order is retained, including inside containers and sequence diagrams.

The immutable `D2Diagram` API works well when composing values, while `D2DiagramBuilder` provides a mutable fluent path. Collection initializer syntax is supported by containers and typed special shapes.

```csharp
var service = new D2Shape("service", "User service", Shape.Rectangle)
{
    new D2Shape("cache", "Cache", Shape.Cylinder),
    new D2Comment("Connections remain in this exact position"),
    new D2Connection("cache", "database", Direction.To, "miss"),
};

var diagram = new D2Diagram(new D2Statement[]
{
    new D2Property("direction", "right"),
    service,
});
```

## Typed D2 features

SQL tables, UML classes, and sequence diagrams have dedicated helpers:

```csharp
var users = new D2SqlTable("users", "Users")
{
    new D2SqlColumn("id", "int", D2SqlConstraint.PrimaryKey),
    new D2SqlColumn("email", "string", D2SqlConstraint.Unique),
};

var service = new D2Class("user_service", "User service")
{
    new D2ClassField("repository", "Repository", D2Visibility.Private),
    new D2ClassMethod(
        "find",
        "User",
        D2Visibility.Public,
        new D2ClassParameter("id", "int")),
};

var request = new D2SequenceDiagram("request", "Find user")
    .AddParticipant("client", "Client", Shape.Person)
    .AddParticipant("service", "Service")
    .AddMessage("client", "service", "find(42)")
    .AddMessage("service", "client", "User", Direction.From);

var diagram = new D2Diagram(new D2Statement[] { users, service, request });
```

All style settings are optional, so named arguments keep declarations compact:

```csharp
var api = new D2Shape(
    "api",
    Shape: Shape.Rectangle,
    Style: new D2Style(
        Stroke: "#e76f51",
        Fill: "#f4a261",
        Opacity: 0.9,
        BorderRadius: 8,
        Font: D2Font.Mono,
        Bold: true))
{
    Link = "https://example.com/docs#api",
    Tooltip = "Open API docs",
    Width = 240,
    Height = 120,
};
```

## Safe serialization and raw D2

Public model values are serialized as data. Identifiers, labels, colors, URLs, tooltips, property values, and other user-controlled strings are quoted and escaped when D2 syntax requires it. Numeric values use invariant formatting, and dotted references preserve D2 path semantics while escaping each segment.

Use `D2Property` for safely serialized scalar or block properties:

```csharp
var config = new D2Property("vars", new D2Statement[]
{
    new D2Property("d2-config", new D2Statement[]
    {
        new D2Property("theme-id", 300),
        new D2Property("center", true),
    }),
});
```

`D2RawStatement` is the deliberate escape hatch for syntax that is not yet modeled. Its contents are emitted without escaping, so create raw statements only from trusted source—not from user input.

```csharp
var import = new D2RawStatement("...@architecture.d2");
```

For Markdown, LaTeX, and other D2 block strings, use `D2Text`. It normalizes line endings and automatically lengthens the pipe delimiter when the contents would otherwise close the block.

## Supported features

- Shapes, nested containers, the modeled D2 shape kinds, icons, dimensions, links, tooltips, and relative placement
- Connections with every arrow direction, labels, icons, links, tooltips, styles, and ordered body properties
- The documented typed style catalog, including colors, opacity, patterns, typography, borders, and animation
- Markdown, LaTeX, code, and other formatted block strings
- Typed SQL tables, constraints, UML classes, members, parameters, and visibility
- Ordered sequence-diagram participants, messages, and groups
- Layers, scenarios, and steps through composition boards
- Comments and root or nested scalar/block properties
- An explicit raw-source escape hatch for advanced D2 syntax

## Roadmap

The general statement/property model can already represent most D2 constructs. Planned work focuses on richer typed helpers for imports, variables, globs, and configuration; more typed sequence-diagram constructs; and deeper parser-backed compatibility coverage as D2 evolves.

Rendering and layout remain the responsibility of D2 itself. A renderer-process wrapper may be provided separately in the future, but it is intentionally outside the core source-model package.

## Development

Build and test with the .NET SDK selected by `global.json`:

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

Parser-backed tests use `d2 validate` when the D2 CLI is available and are reported as inconclusive when it is not installed.

## Inspiration and thanks

- [Kreshnik/d2lang-js](https://github.com/Kreshnik/d2lang-js)
- [MrBlenny/py-d2](https://github.com/MrBlenny/py-d2)

If this project is useful to you, you can [support its development](https://www.buymeacoffee.com/stephanvs).

Copyright © 2023–present Stephan van Stekelenburg. Provided under the [MIT License](LICENSE).
