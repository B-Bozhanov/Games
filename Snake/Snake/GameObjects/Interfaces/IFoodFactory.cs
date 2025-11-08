namespace SnakeGame.GameObjects.Interfaces
{
    public interface IFoodFactory
    {
        public Food GetFood(Coordinates boardSize, IReadOnlyCollection<Coordinates> snakeBody);
    }
}
