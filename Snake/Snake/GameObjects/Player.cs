namespace SnakeGame.GameObjects
{
    using SnakeGame.Core.State;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public sealed class Player(SnakeId snakeId, ISnake snake, string name, PlayerType type)
    {
        public SnakeId Id { get; } = snakeId;

        public Guid Id1 { get; } = Guid.NewGuid();

        public ISnake Snake { get; } = snake;

        public string Name { get; } = name;

        public PlayerType Type { get; } = type;

        public bool IsAlive { get; set; } = true;

        public double MoveIntervalSeconds { get; set; } = 0.15;

        public double MoveTimer { get; set; } = 0;

        public int Score { get; set; }
    }
}

