namespace SnakeGame.GameObjects.Abstractions.Base
{
    using SnakeGame.GameObjects.Enums;

    public abstract class BaseItem(Coordinates coordinates, TimeSpan lifeTime)
    {
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public Coordinates Coordinates { get; set; } = coordinates;

        public TimeSpan LifeTime { get; set; } = lifeTime;

        public bool IsExpired => DateTime.UtcNow - StartTime > LifeTime;

        public abstract Color Color { get; protected set; }
    }
}