namespace SnakeGame.Core.Scenes
{
    using SnakeGame.Core.Scenes.Interfaces;

    public sealed class GameTime : IGameTime
    {
        // === CONFIG ===
        // Update rate за FixedUpdate (много важно за multiplayer)
        private const double FixedStepSeconds = 1.0 / 60.0; // 60 ticks/second

        // Може да го направиш и 60 → 1.0 / 60.0

        // === TIME TRACKING ===
        private DateTime lastFrame;

        private DateTime fpsTimer;

        // Delta между два кадъра (render loop)
        public double DeltaTimeSeconds { get; private set; }

        // Натрупване за fixed tick
        private double accumulator = 0;

        // FPS counter
        public int CurrentFps { get; private set; }

        private int fpsCount = 0;

        public GameTime()
        {
            var now = DateTime.UtcNow;
            lastFrame = now;
            fpsTimer = now;
        }

        /// <summary>
        /// Извиква се В НАЧАЛОТО на всеки кадър.
        /// Пресмята DeltaTime и управлява FPS.
        /// </summary>
        public void Tick()
        {
            var now = DateTime.UtcNow;

            // 1) Real-time delta
            DeltaTimeSeconds = (now - lastFrame).TotalSeconds;
            lastFrame = now;

            // 2) FPS брояч
            fpsCount++;
            if ((now - fpsTimer).TotalSeconds >= 1)
            {
                CurrentFps = fpsCount;
                fpsCount = 0;
                fpsTimer = now;
            }

            // 3) Натрупваме време за FixedUpdate
            accumulator += DeltaTimeSeconds;
        }

        /// <summary>
        /// Изпълнява fixed update ticks, ако е време.
        /// Връща true за всяко фиксирано изпълнение.
        /// </summary>
        public bool ShouldDoFixedUpdate()
        {
            if (accumulator >= FixedStepSeconds)
            {
                accumulator -= FixedStepSeconds;
                return true;
            }

            return false;
        }

        public double FixedDeltaSeconds => FixedStepSeconds;
    }
}