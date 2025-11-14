namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    public interface IFoodFactory
    {
        public Food CreateFood(IBoardConfig boardConfig, IReadOnlyCollection<Coordinates> blockedPositions);


        public IReadOnlyCollection<Obstacle> CreateObstacles(int count, IBoardConfig boardConfig,
               IReadOnlyCollection<Coordinates> blockedPositions);
    }
}
