namespace SnakeGame.Core.State
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public sealed class SnakeState
    {
        public SnakeState(SnakeId id, IEnumerable<Coordinates> body, Direction initialDirection)
        {
            this.Id = id;
            this.Body = new Queue<Coordinates>(body);
            this.InitialDirection = initialDirection;
            this.IsAlive = true;
            this.MoveIntervalSeconds = 0.15; // Default move interval
        }

        public SnakeId Id { get; }

        public Queue<Coordinates> Body { get; }

        public Direction InitialDirection { get; }

        public bool IsAlive { get; set; }

        public double MoveIntervalSeconds { get; set; }

        public int Score { get; set; }
    }
}
