namespace SnakeGame.GameObjects
{
    using System;
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public class FoodFactory : IFoodFactory
    {
        private readonly Random random;
        private readonly int startSeconds;
        private readonly int endSeconds;
        private TimeSpan objectLifeTimeSeconds;

        public FoodFactory()
        {
            this.startSeconds = 10;
            this.endSeconds = 30;
            this.random = new Random();
        }

        public Food CreateFood(IBoardConfig boardConfig, IReadOnlyCollection<Coordinates> blockedPositions)
        {
            //ValidateBoard(GameStartRow, GameEndRow, GameStartCol, GameEndCol);
            var foodCoordinates = this.GetRandomFreePosition(boardConfig, blockedPositions);
            this.objectLifeTimeSeconds = TimeSpan.FromSeconds(this.random.Next(this.startSeconds, this.endSeconds));

            return new Food(foodCoordinates, objectLifeTimeSeconds); 
        }

        public IReadOnlyCollection<Obstacle> CreateObstacles(int count, IBoardConfig boardConfig,
               IReadOnlyCollection<Coordinates> blockedPositions)
        {
            throw new NotImplementedException();
        }

        private Coordinates GetRandomFreePosition(IBoardConfig boardConfig, IReadOnlyCollection<Coordinates> blockedPositions)
        {
            while (true)
            {
                int row = this.random.Next(boardConfig.PlayableStartRow, boardConfig.PlayableEndRow);
                int col = this.random.Next(boardConfig.PlayableStartCol, boardConfig.PlayableEndCol);
                var candidate = new Coordinates(row, col);
                bool isBlocked = blockedPositions.Any(c => c == candidate);
                if (isBlocked) continue;

                return candidate;
            }
        }

        private static void ValidateBoard(CellType[,] board)
        {
            //if (boardStartRows >= boardEndRows || boardStartCol >= boardEndCol)
            //{
            //    throw new InvalidOperationException("Start value canot be greater than end value");
            //}
        }
    }
}
