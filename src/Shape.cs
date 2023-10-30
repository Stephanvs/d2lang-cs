namespace d2;

public abstract record Shape(string Value)
{
  public readonly static Rectangle Rectangle = new();
  public readonly static Square Square = new();
  public readonly static Page Page = new();
  public readonly static Parallelogram Parallelogram = new();
  public readonly static Document Document = new();
  public readonly static Cylinder Cylinder = new();
  public readonly static Queue Queue = new();
  public readonly static Package Package = new();
  public readonly static Step Step = new();
  public readonly static Callout Callout = new();
  public readonly static StoredData StoredData = new();
  public readonly static Person Person = new();
  public readonly static Diamond Diamond = new();
  public readonly static Oval Oval = new();
  public readonly static Circle Circle = new();
  public readonly static Hexagon Hexagon = new();
  public readonly static Cloud Cloud = new();
  public readonly static Text Text = new();
  public readonly static Code Code = new();
  public readonly static SqlTable SqlTable = new();
  public readonly static Image Image = new();
  public readonly static Class Class = new();
  public readonly static SequenceDiagram SequenceDiagram = new();

  public sealed override string ToString() => Value;
}

public sealed record Rectangle() : Shape("rectangle");
public sealed record Square() : Shape("square");
public sealed record Page() : Shape("page");
public sealed record Parallelogram() : Shape("parallelogram");
public sealed record Document() : Shape("document");
public sealed record Cylinder() : Shape("cylinder");
public sealed record Queue() : Shape("queue");
public sealed record Package() : Shape("package");
public sealed record Step() : Shape("step");
public sealed record Callout() : Shape("callout");
public sealed record StoredData() : Shape("stored_data");
public sealed record Person() : Shape("person");
public sealed record Diamond() : Shape("diamond");
public sealed record Oval() : Shape("oval");
public sealed record Circle() : Shape("circle");
public sealed record Hexagon() : Shape("hexagon");
public sealed record Cloud() : Shape("cloud");
public sealed record Text() : Shape("text");
public sealed record Code() : Shape("code");
public sealed record SqlTable() : Shape("sql_table");
public sealed record Image() : Shape("image");
public sealed record Class() : Shape("class");
public sealed record SequenceDiagram() : Shape("sequence_diagram");
