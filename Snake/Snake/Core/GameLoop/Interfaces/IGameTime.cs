namespace SnakeGame.Core.GameLoop.Interfaces
{
    public interface IGameTime
    {
        int CurrentFps { get; }
        double DeltaTimeSeconds { get; }
        double FixedDeltaSeconds { get; }

        bool ShouldDoFixedUpdate();
        void Tick();
    }
}