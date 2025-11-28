namespace SnakeGame.Core.Scenes.Interfaces
{
    using SnakeGame.Core.State;

    public interface IGameEngine
    {
        void FixedUpdate(GameState state, double deltaSeconds);
    }
}
