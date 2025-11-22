namespace SnakeGame.GameObjects
{
    using System;
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Abstractions.Interfaces;

    public class BaseObjectFactory : IObjectFactory
    {
        private readonly Random random;
        private readonly int startSeconds;
        private readonly int endSeconds;
        private TimeSpan objectLifeTimeSeconds;

        public BaseObjectFactory()
        {
            this.startSeconds = 10;
            this.endSeconds = 30;
            this.random = new Random();
        }

        public Food CreateFood(IBoardConfig boardConfig, bool[,] blockList)
        {
            var foodCoordinates = this.GetRandomFreePosition(boardConfig, blockList);
            this.objectLifeTimeSeconds = TimeSpan.FromSeconds(this.random.Next(this.startSeconds, this.endSeconds));

            return new Food(foodCoordinates, objectLifeTimeSeconds);
        }

        public IDictionary<Coordinates, Obstacle> CreateObstacles(int count, IBoardConfig boardConfig, bool[,] blockList)
        {
            if (count <= 0) throw new InvalidOperationException("Count must be greater than 0");

            var obstacles = new Dictionary<Coordinates, Obstacle>();

            for (int o = 0; o < count; o++)
            {
                var coordinates = this.GetRandomFreePosition(boardConfig, blockList);
                this.objectLifeTimeSeconds = TimeSpan.FromSeconds(this.random.Next(this.startSeconds, this.endSeconds));
                var obstacle = new Obstacle(coordinates, this.objectLifeTimeSeconds);

                if (!obstacles.ContainsKey(obstacle.Coordinates))
                {
                    obstacles.Add(obstacle.Coordinates, obstacle);
                }
            }

            return obstacles;
        }

        private Coordinates GetRandomFreePosition(IBoardConfig boardConfig, bool[,] blockList)
        {
            while (true)
            {
                int row = this.random.Next(boardConfig.PlayableStartRow, boardConfig.TotalRows - 1);
                int col = this.random.Next(boardConfig.PlayableStartCol, boardConfig.TotalCols - 1);
                var candidate = new Coordinates(row, col);
                var isBlocked = blockList[candidate.Row, candidate.Col];
                if (isBlocked) continue;

                return candidate;
            }
        }
    }
}