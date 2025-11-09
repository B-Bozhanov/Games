namespace SnakeGame.Rendering
{
    using SnakeGame.GameObjects;

    public interface IRenderer
    {
        void ClearElement(Coordinates element);

        public void DrowFood(Coordinates food, char symbol);

        public void DrowSnake(IReadOnlyCollection<Coordinates> snakeBody);
    }
}
