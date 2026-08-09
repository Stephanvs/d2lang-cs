using System.Collections;

namespace d2;

public enum D2BoardKind
{
  Layers,
  Scenarios,
  Steps,
}

/// <summary>
/// A D2 composition block containing layers, scenarios, or steps.
/// </summary>
public sealed record D2BoardCollection : D2Statement, IEnumerable<D2Board>
{
  private readonly List<D2Board> _boards;

  public D2BoardKind Kind { get; }

  public IReadOnlyList<D2Board> Boards => _boards;

  public D2BoardCollection(D2BoardKind kind)
    : this(kind, Array.Empty<D2Board>())
  {
  }

  public D2BoardCollection(D2BoardKind kind, IEnumerable<D2Board> boards)
  {
    if (!Enum.IsDefined(typeof(D2BoardKind), kind))
    {
      throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown D2 board kind.");
    }

    if (boards is null)
    {
      throw new ArgumentNullException(nameof(boards));
    }

    Kind = kind;
    _boards = boards.ToList();
    if (_boards.Any(board => board is null))
    {
      throw new ArgumentException("A board collection cannot contain null.", nameof(boards));
    }
  }

  public void Add(D2Board board)
  {
    if (board is null)
    {
      throw new ArgumentNullException(nameof(board));
    }

    _boards.Add(board);
  }

  internal override IEnumerable<string> Lines()
    => D2Writer.Block(Keyword(Kind), _boards.SelectMany(board => board.Lines()));

  public override string ToString() => string.Join(Environment.NewLine, Lines());

  public IEnumerator<D2Board> GetEnumerator() => _boards.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  private static string Keyword(D2BoardKind kind) => kind switch
  {
    D2BoardKind.Layers => "layers",
    D2BoardKind.Scenarios => "scenarios",
    D2BoardKind.Steps => "steps",
    _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown D2 board kind."),
  };
}
