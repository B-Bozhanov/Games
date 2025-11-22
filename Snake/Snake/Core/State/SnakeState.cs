namespace SnakeGame.Core.State
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public sealed class SnakeState(SnakeId id, IEnumerable<Coordinates> body, Direction initialDirection)
    {
        public SnakeId Id { get; } = id;

        public Queue<Coordinates> Body { get; } = new Queue<Coordinates>(body);

        public Direction CurrentDirection { get; set; } = initialDirection;

        public bool IsAlive { get; set; } = true;

        public double MoveIntervalSeconds { get; set; } = 0.15; // Default move interval

        public int Score { get; set; }
    }
}
