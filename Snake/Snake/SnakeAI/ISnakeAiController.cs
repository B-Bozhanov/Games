namespace SnakeGame.SnakeAI
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public interface ISnakeAiController
    {
        public Direction GetNextDirection(
            IGameBoard gameBoard,
            Coordinates head,
            Coordinates food,
            IReadOnlyCollection<Coordinates> body);
    }
}
