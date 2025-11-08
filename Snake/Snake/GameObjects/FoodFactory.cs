namespace SnakeGame.GameObjects
{
    using System;
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Interfaces;

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
            while (true)
            {
                int x = this.random.Next(0, boardSize.Row - 1);
                int y = this.random.Next(0, boardSize.Col - 1);

                bool isOnSnake = snakeBody.Any(c => c.Row == x && c.Col == y);

                if (isOnSnake) continue;

                this.foodLifeTimeSeconds = TimeSpan.FromSeconds(this.random.Next(this.startSeconds, this.endSeconds));
                var foodCoordinates = new Coordinates(x, y);
                var food = new Food(foodCoordinates, foodLifeTimeSeconds);

                return food;
            }
        }
    }
}
