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
                int x = this.random.Next(0, boardSize.Row - 3);
                int y = this.random.Next(0, boardSize.Col - 3);

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
