namespace SnakeGame.Core
{
    public class GameTime
    {
        private readonly int targetFrameTimeMs;
        private DateTime lastFrameTime;
        private DateTime startTime;

        public GameTime(int targetFps = 10)
        {
            this.targetFrameTimeMs = 1000 / targetFps;
            this.startTime = DateTime.UtcNow;
            this.lastFrameTime = this.startTime;
        }

        public TimeSpan TotalTime => DateTime.UtcNow - this.startTime;

        public TimeSpan MiddleTime { get; private set; }

        public void Tick()
        {
            var now = DateTime.UtcNow;
            this.MiddleTime = now - this.lastFrameTime;

            int sleepTime = this.targetFrameTimeMs - (int)this.MiddleTime.TotalMilliseconds;
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
