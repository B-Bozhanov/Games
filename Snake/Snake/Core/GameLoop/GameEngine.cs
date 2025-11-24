namespace SnakeGame.Core.GameLoop
{
    using System.Collections.Generic;

    using SnakeGame.Core.GameLoop.Interfaces;
    using SnakeGame.Core.State;
    using SnakeGame.GameObjects.Enums;

    public sealed class GameEngine : IGameEngine
    {
        public void FixedUpdate(GameState state, IReadOnlyDictionary<SnakeId, Direction> decisions, double deltaSeconds)
        {
            throw new NotImplementedException();
        }
    }
}
