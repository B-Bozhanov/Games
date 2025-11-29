namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    public interface IObjectFactory
    {
        public Food CreateFood(IBoardConfig boardConfig, bool[,] blockList);

        public IDictionary<Coordinates, Obstacle> CreateObstacles(int count, IBoardConfig boardConfig, bool[,] blockList);
    }
}