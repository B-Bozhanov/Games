namespace SnakeGame.Core
{
    public interface IGameTime
    {
        public int TargetFrameTimeMs { get; }

        public TimeSpan MiddleTime { get; }

        public void IncreaseSpeed();

        public void Tick();
    }
}
