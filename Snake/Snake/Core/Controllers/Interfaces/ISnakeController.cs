namespace SnakeGame.Core.Controllers.Interfaces
{
    using SnakeGame.Core.State;
    using SnakeGame.GameObjects.Enums;

    public interface ISnakeController
    {
        public interface ISnakeController
        {
            public Direction? GetNextDirection(GameState state, SnakeId snakeId, double nowSeconds);
        }
    }
}
