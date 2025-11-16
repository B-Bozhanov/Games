namespace SnakeGame.Core
{
    using SnakeGame.Common;

    public class GameTime : IGameTime
    {
        private readonly int defaultTargetFps = GlobalConstants.GameConstants.GameDefaultFps;
        private int currentFps;

        private DateTime lastFrameTime;
        private DateTime startTime;

        public GameTime()
        {
            this.currentFps = this.defaultTargetFps;
            this.startTime = DateTime.UtcNow;
            this.lastFrameTime = this.startTime;
        }

        public TimeSpan TotalTime => DateTime.UtcNow - this.startTime;

        public int CurrentFps => this.currentFps;

        public int TargetFrameTimeMs => 1000 / this.currentFps;

        public TimeSpan MiddleTime { get; private set; }

        public void IncreaseSpeed()
        {
            this.currentFps++;// (int)(this.currentFps * 1.05);
        }

        public void Tick()
        {
            var now = DateTime.UtcNow;
            this.MiddleTime = now - this.lastFrameTime;

            int sleepTime = this.TargetFrameTimeMs - (int)this.MiddleTime.TotalMilliseconds;
            if (sleepTime > 0)
            {
                Thread.Sleep(sleepTime);
                now = DateTime.UtcNow;
                this.MiddleTime = now - this.lastFrameTime;
            }

            this.lastFrameTime = now;
        }
    }
}
