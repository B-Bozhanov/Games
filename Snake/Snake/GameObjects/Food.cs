namespace SnakeGame.GameObjects
{
    using SnakeGame.GameObjects.Enums;

    public sealed class Food(Coordinates coordinates, TimeSpan lifeTime)
    {
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public Coordinates Coordinates { get; set; } = coordinates;

        public TimeSpan LifeTime { get; set; } = lifeTime;

        public bool IsExpired => DateTime.UtcNow - StartTime > LifeTime;
    }
}
