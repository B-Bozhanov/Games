namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface ISnakeAiController
    {
        public Direction GetNextDirection(SnakeAiContext context);
    }
}
