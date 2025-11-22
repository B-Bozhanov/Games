namespace SnakeGame.GameObjects
{
    using SnakeGame.Core.State;
    using SnakeGame.GameObjects.Enums;

    public class Player(int id, string name, SnakeId snakeId, PlayerType type)
    {
        public int Id { get; } = id;

        public string Name { get; } = name;

        public SnakeId SnakeId { get; } = snakeId;

        public PlayerType Type { get; } = type;

        public double MoveIntervalSeconds { get; set; } = 0.15;

        public int Score { get; set; }
    }
}
