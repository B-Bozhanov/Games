namespace SnakeGame.Core
{
    public interface IGameTime
    {
        public int TargetFrameTimeMs { get; }

        public TimeSpan MiddleTime { get; }

        public int CurrentFps { get; }


        public void IncreaseSpeed();

        public void Tick();
    }
}
