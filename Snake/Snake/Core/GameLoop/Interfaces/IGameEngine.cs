namespace SnakeGame.Core.GameLoop.Interfaces
{
    using SnakeGame.Core.State;

    public interface IGameEngine
    {
        void FixedUpdate(GameState state, double deltaSeconds);
    }
}
