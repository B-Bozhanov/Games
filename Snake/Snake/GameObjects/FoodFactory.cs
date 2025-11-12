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
            while (true)
            {
                int row = this.random.Next(HeaderHeight, boardSize.Row -1);
                int col = this.random.Next(0, boardSize.Col);
                foodCoordinates.Row = row;
                foodCoordinates.Col = col;
                bool isOnSnake = snakeBody.Any(c => c.Row == row && c.Col == col);

                if (!foodCoordinates.IsInRange(GameHeight, GameWidth))
                {
                    Console.WriteLine("Bug");
                    continue;
                }
                if (isOnSnake) continue;

                this.foodLifeTimeSeconds = TimeSpan.FromSeconds(this.random.Next(this.startSeconds, this.endSeconds));
                var food = new Food(foodCoordinates, foodLifeTimeSeconds);

                return food;
            }
        }
    }
}
