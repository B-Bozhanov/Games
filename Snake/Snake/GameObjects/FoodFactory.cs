namespace SnakeGame.GameObjects
{
    using System;
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Interfaces;

    using static SnakeGame.Common.GlobalConstants.GameConstants;
    public class FoodFactory : IFoodFactory
    {
        private readonly Random random;
        private readonly int startSeconds;
        private readonly int endSeconds;
        private TimeSpan foodLifeTimeSeconds;

        public FoodFactory()
        {
            this.startSeconds = 10;
            this.endSeconds = 30;
            this.random = new Random();
        }

        public Food GetFood(Coordinates boardSize, IReadOnlyCollection<Coordinates> snakeBody)
        {
            var foodCoordinates = new Coordinates();
            var boardStartRows = HeaderHeight + 1;
            var boardEndRows = boardSize.Row - WallsWidth - WallsWidth;
            var boardStartCol = WallsWidth;
            var boardEndCol = boardSize.Col - WallsWidth - WallsWidth;

            if (boardStartRows >= boardEndRows || boardStartCol >= boardEndCol)
            {
                throw new InvalidOperationException("Start value canot be greater than end value");
            }

            while (true)
            {
                int row = this.random.Next(boardStartRows, boardEndRows);
                int col = this.random.Next(boardStartCol, boardEndCol);
                foodCoordinates.Row = row;
                foodCoordinates.Col = col;
                bool isOnSnake = snakeBody.Any(c => c == foodCoordinates);

                if (isOnSnake) continue;

                this.foodLifeTimeSeconds = TimeSpan.FromSeconds(this.random.Next(this.startSeconds, this.endSeconds));
                var food = new Food(foodCoordinates, foodLifeTimeSeconds);

                return food;
            }
        }
    }
}
