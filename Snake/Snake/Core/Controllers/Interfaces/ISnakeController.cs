namespace SnakeGame.Core.Controllers.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface ISnakeController
    {
        public Direction GetNextDirection(GetNextDirectionsContext context);
    }
}