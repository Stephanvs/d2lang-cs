# d2lang-cs

![Banner](docs/assets/img/banner.png)

An unofficial interface for building [D2](https://github.com/terrastruct/d2) diagram files in C# and dotnet.

# Installation

```bash
dotnet add package d2lang-cs
```

# Usage

```csharp
using d2;

var umbrella = new D2Shape("alphabet", "Alphabet Inc", Shape.Rectangle);
var company = new D2Shape("google", null, Shape.Rectangle);

company.Add(new D2Shape("gmail", "Gmail", Shape.Rectangle));
company.Add(new D2Shape("meet", "Meet", Shape.Rectangle));
company.Add(new D2Shape("deepmind", "DeepMind", Shape.Rectangle));

company.Icon = "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_92x30dp.png";

var connection = new D2Connection(company.Name, umbrella.Name, Direction.To, "BELONGS_TO");

var diagram = new D2Diagram(new[] { umbrella, company }, new[] { connection });

Console.WriteLine(diagram.ToString());
```

# D2 Output
```d2-lang
alphabet: Alphabet Inc {
  shape: rectangle
}
google: {
  gmail: Gmail {
    shape: rectangle
  }
  meet: Meet {
    shape: rectangle
  }
  deepmind: DeepMind {
    shape: rectangle
  }
  icon: https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_92x30dp.png
  shape: rectangle
}
google -> alphabet: BELONGS_TO
```

# Diagram Output
![Diagram](docs/assets/img/diagram.png)

# Documentation

## Typed diagrams

Special D2 shapes have typed helpers while still participating in the ordered
`D2Statement` model:

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
    .AddParticipant("client", "Client")
    .AddParticipant("service", "Service")
    .AddMessage("client", "service", "find(42)")
    .AddMessage("service", "client", "User", Direction.From);

var diagram = new D2Diagram(new D2Statement[] { users, service, request });
```

All style arguments are optional and named arguments keep declarations compact:

```csharp
var api = new D2Shape(
    "api",
    Shape: Shape.Rectangle,
    Style: new D2Style(
        Fill: "#f4a261",
        BorderRadius: 8,
        Font: D2Font.Mono,
        Animated: true))
{
    Link = "https://example.com/docs#api",
    Tooltip = "Open API docs",
    Width = 240,
    Height = 120,
};
```

## Supported
- [x] Shapes (nodes)
- [x] Connections (edges)
- [x] Full documented style catalog
- [x] Containers (nodes/links in nodes)
- [x] Arrow directions
- [x] Markdown / latex / block strings / code in shapes
- [x] Shape icons
- [x] Shape dimensions, links, and tooltips
- [x] Connection icons, styles, links, and tooltips
- [x] Typed SQL table shapes and constraints
- [x] Typed UML classes, members, parameters, and visibility
- [x] Ordered sequence-diagram helpers
- [x] Comments, root properties, composition boards, and raw escape hatches

# Inspiration & Thanks
- [Kreshnik/d2lang-js](https://github.com/Kreshnik/d2lang-js)
- [MrBlenny/py-d2](https://github.com/MrBlenny/py-d2)

# Thank me!
If you like what I'm doing and you would like to thank me, please consider:

<a href="https://www.buymeacoffee.com/stephanvs" target="_blank">
<img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee!" style="height: 60px !important;width: 217px !important;" >
</a>

Thank you for your support!

<hr />

Copyright &copy; 2023 [Stephan van Stekelenburg](https://stephanvs.com) - Provided under [MIT License](LICENSE)
