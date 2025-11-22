namespace SnakeGame.SnakeAI
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;

    public class SnakeAiContext(
        Coordinates snakeHead,
        IReadOnlyCollection<Coordinates> snakeBody,
        Coordinates food,
        IGameBoard gameBoard)
    {
        public IGameBoard GameBoard => gameBoard;

        public Coordinates Head { get; set; }

        public Coordinates Food { get; set; }

        public HashSet<Coordinates> Body { get; set; }
    }
}