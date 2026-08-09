namespace d2;

/// <summary>Represents a D2 shape kind.</summary>
/// <param name="Value">The D2 shape keyword.</param>
public abstract record Shape(string Value)
{
  /// <summary>The default rectangular shape.</summary>
  public readonly static Rectangle Rectangle = new();
  /// <summary>A square shape.</summary>
  public readonly static Square Square = new();
  /// <summary>A page shape.</summary>
  public readonly static Page Page = new();
  /// <summary>A parallelogram shape.</summary>
  public readonly static Parallelogram Parallelogram = new();
  /// <summary>A document shape.</summary>
  public readonly static Document Document = new();
  /// <summary>A cylinder shape, commonly used for data stores.</summary>
  public readonly static Cylinder Cylinder = new();
  /// <summary>A queue shape.</summary>
  public readonly static Queue Queue = new();
  /// <summary>A package shape.</summary>
  public readonly static Package Package = new();
  /// <summary>A step shape.</summary>
  public readonly static Step Step = new();
  /// <summary>A callout shape.</summary>
  public readonly static Callout Callout = new();
  /// <summary>A stored-data shape.</summary>
  public readonly static StoredData StoredData = new();
  /// <summary>A person shape.</summary>
  public readonly static Person Person = new();
  /// <summary>A diamond shape.</summary>
  public readonly static Diamond Diamond = new();
  /// <summary>An oval shape.</summary>
  public readonly static Oval Oval = new();
  /// <summary>A circle shape.</summary>
  public readonly static Circle Circle = new();
  /// <summary>A hexagon shape.</summary>
  public readonly static Hexagon Hexagon = new();
  /// <summary>A cloud shape.</summary>
  public readonly static Cloud Cloud = new();
  /// <summary>A text-only shape.</summary>
  public readonly static Text Text = new();
  /// <summary>A code shape.</summary>
  public readonly static Code Code = new();
  /// <summary>A SQL-table shape.</summary>
  public readonly static SqlTable SqlTable = new();
  /// <summary>An image shape.</summary>
  public readonly static Image Image = new();
  /// <summary>A UML class shape.</summary>
  public readonly static Class Class = new();
  /// <summary>A sequence-diagram container shape.</summary>
  public readonly static SequenceDiagram SequenceDiagram = new();

  /// <inheritdoc />
  public sealed override string ToString() => Value;
}

/// <summary>Represents D2's <c>rectangle</c> shape.</summary>
public sealed record Rectangle() : Shape("rectangle");
/// <summary>Represents D2's <c>square</c> shape.</summary>
public sealed record Square() : Shape("square");
/// <summary>Represents D2's <c>page</c> shape.</summary>
public sealed record Page() : Shape("page");
/// <summary>Represents D2's <c>parallelogram</c> shape.</summary>
public sealed record Parallelogram() : Shape("parallelogram");
/// <summary>Represents D2's <c>document</c> shape.</summary>
public sealed record Document() : Shape("document");
/// <summary>Represents D2's <c>cylinder</c> shape.</summary>
public sealed record Cylinder() : Shape("cylinder");
/// <summary>Represents D2's <c>queue</c> shape.</summary>
public sealed record Queue() : Shape("queue");
/// <summary>Represents D2's <c>package</c> shape.</summary>
public sealed record Package() : Shape("package");
/// <summary>Represents D2's <c>step</c> shape.</summary>
public sealed record Step() : Shape("step");
/// <summary>Represents D2's <c>callout</c> shape.</summary>
public sealed record Callout() : Shape("callout");
/// <summary>Represents D2's <c>stored_data</c> shape.</summary>
public sealed record StoredData() : Shape("stored_data");
/// <summary>Represents D2's <c>person</c> shape.</summary>
public sealed record Person() : Shape("person");
/// <summary>Represents D2's <c>diamond</c> shape.</summary>
public sealed record Diamond() : Shape("diamond");
/// <summary>Represents D2's <c>oval</c> shape.</summary>
public sealed record Oval() : Shape("oval");
/// <summary>Represents D2's <c>circle</c> shape.</summary>
public sealed record Circle() : Shape("circle");
/// <summary>Represents D2's <c>hexagon</c> shape.</summary>
public sealed record Hexagon() : Shape("hexagon");
/// <summary>Represents D2's <c>cloud</c> shape.</summary>
public sealed record Cloud() : Shape("cloud");
/// <summary>Represents D2's <c>text</c> shape.</summary>
public sealed record Text() : Shape("text");
/// <summary>Represents D2's <c>code</c> shape.</summary>
public sealed record Code() : Shape("code");
/// <summary>Represents D2's <c>sql_table</c> shape.</summary>
public sealed record SqlTable() : Shape("sql_table");
/// <summary>Represents D2's <c>image</c> shape.</summary>
public sealed record Image() : Shape("image");
/// <summary>Represents D2's <c>class</c> shape.</summary>
public sealed record Class() : Shape("class");
/// <summary>Represents D2's <c>sequence_diagram</c> shape.</summary>
public sealed record SequenceDiagram() : Shape("sequence_diagram");
