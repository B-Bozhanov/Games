namespace SnakeGame.Core.State;

using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Abstractions.Interfaces;

public sealed class GameState(IGameBoard gameBoard)
{

    public IGameBoard GameBoard { get; set; } = gameBoard;

    public IBoardConfig BoardConfig { get; } = gameBoard.BoardConfig;

    public IDictionary<SnakeId, Player> Players { get; set; } = new Dictionary<SnakeId, Player>();

    public IDictionary<Coordinates, Obstacle> Obstacles { get; set; } = new Dictionary<Coordinates, Obstacle>();

    public Food? Food { get; set; }

    public bool[,] BlockList { get; } = new bool[gameBoard.BoardConfig.TotalRows, gameBoard.BoardConfig.TotalCols];

    public long TickCount { get; set; }

    public bool IsGameOver { get; set; }

    public SnakeId? WinnerSnakeId { get; set; }

    public bool IsBlocked(Coordinates coordinates)
      => this.BlockList[coordinates.Row, coordinates.Col];

    public void UnBlock(Coordinates coordinates)
               => this.BlockList[coordinates.Row, coordinates.Col] = false;

    public void UnBlock(IReadOnlyCollection<Coordinates> coordinates)
    {
        foreach (var c in coordinates)
        {
            this.BlockList[c.Row, c.Col] = false;
        }
    }

    public void Block(Coordinates coordinates)
       => this.BlockList[coordinates.Row, coordinates.Col] = true;

    public void Block(IReadOnlyCollection<Coordinates> coordinates)
    {
        foreach (var c in coordinates)
        {
            this.BlockList[c.Row, c.Col] = true;
        }
    }
}
