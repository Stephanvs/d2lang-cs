using System.Collections;

namespace d2;

/// <summary>Identifies a D2 composition-board collection.</summary>
public enum D2BoardKind
{
  /// <summary>A collection of alternate diagram layers.</summary>
  Layers,
  /// <summary>A collection of scenarios based on a board.</summary>
  Scenarios,
  /// <summary>An ordered collection of board steps.</summary>
  Steps,
}

/// <summary>
/// A D2 composition block containing layers, scenarios, or steps.
/// </summary>
public sealed record D2BoardCollection : D2Statement, IEnumerable<D2Board>
{
  private readonly List<D2Board> _boards;

  /// <summary>Gets the kind of boards contained by this collection.</summary>
  public D2BoardKind Kind { get; }

  /// <summary>Gets the boards in their serialization order.</summary>
  public IReadOnlyList<D2Board> Boards => _boards;

  /// <summary>Initializes an empty board collection.</summary>
  /// <param name="kind">The collection kind.</param>
  public D2BoardCollection(D2BoardKind kind)
    : this(kind, Array.Empty<D2Board>())
  {
  }

  /// <summary>Initializes a collection with a sequence of boards.</summary>
  /// <param name="kind">The collection kind.</param>
  /// <param name="boards">The boards to include.</param>
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

  /// <summary>Adds a board to the end of the collection.</summary>
  /// <param name="board">The board to add.</param>
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

  /// <inheritdoc />
  public override string ToString() => string.Join(Environment.NewLine, Lines());

  /// <inheritdoc />
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
