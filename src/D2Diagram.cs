namespace d2;

public record class D2Diagram(
  IEnumerable<D2Shape> Shapes,
  IEnumerable<D2Connection> Connections
)
{
  public D2Diagram Add(D2Shape shape)
    => this with { Shapes = Shapes.Append(shape) };
  public D2Diagram Add(D2Connection connection)
    => this with { Connections = Connections.Append(connection) };

  internal IEnumerable<string> Lines()
  {
    var shapes = Shapes.SelectMany(s => s.Lines()).ToList();
    var connections = Connections.SelectMany(c => c.Lines()).ToList();

    return shapes.Concat(connections);
  }

  public override string ToString()
    => string.Join(Environment.NewLine, Lines());
}