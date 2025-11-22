namespace SnakeGame.Core.State
{
    using SnakeGame.GameObjects;

    public class ObstacleState(Coordinates position, double lifeTimeSeconds)
    {
        public Coordinates Position { get; } = position;

        public double LifeTimeSeconds { get; init; } = lifeTimeSeconds;

        public double AgeSeconds { get; set; } = 0;

        public bool IsExpired => this.AgeSeconds >= this.LifeTimeSeconds;
    }
}