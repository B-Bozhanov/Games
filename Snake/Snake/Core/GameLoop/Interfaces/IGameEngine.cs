namespace SnakeGame.Core.GameLoop.Interfaces
{
    using SnakeGame.Core.State;
    using SnakeGame.GameObjects.Enums;

    public interface IGameEngine
    {
        void FixedUpdate(GameState state, IReadOnlyDictionary<SnakeId, Direction> decisions, double deltaSeconds);
    }
}
