namespace SnakeGame.Core.State
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;

    public class GameState(IBoardConfig boardConfig)
    {
        public IBoardConfig BoardConfig { get; } = boardConfig;

        public bool[,] Occupied { get; set; } = new bool[boardConfig.TotalRows, boardConfig.TotalCols];

        public Dictionary<SnakeId, SnakeState> Snakes { get; } = [];

        public FoodState? Food { get; set; }

        public Dictionary<Coordinates, ObstacleState> Obstacles { get; } = [];

        public long TickCount { get; set; }

        public bool IsGameOver { get; set; } = false;

        public SnakeId? WinnerSnakeId { get; set; }
    }
}
