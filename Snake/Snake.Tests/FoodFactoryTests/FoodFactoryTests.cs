namespace Snake.Tests.FoodFactoryTests
{
    using SnakeGame.GameObjects;

    using Xunit;

    public class FoodFactoryTests
    {
        [Fact]
        public void GetFood_Many_Times_Should_Never_Spawn_On_Snake()
        {
            // Arrange
            var factory = new FoodFactory();
            var boardSize = new Coordinates(20, 40);

            var snakeBody = new List<Coordinates>
        {
            new(5, 5),
            new(5, 6),
            new(5, 7),
            new(10, 10),
            new(15, 20),
        };

            // Act + Assert
            for (int i = 0; i < 100; i++)
            {
                var food = factory.GetFood(boardSize, snakeBody);

                Assert.DoesNotContain(food.Coordinates, snakeBody);
                Assert.InRange(food.Coordinates.Row, 0, boardSize.Row - 1);
                Assert.InRange(food.Coordinates.Col, 0, boardSize.Col - 1);

                Assert.InRange(food.LifeTime.TotalSeconds, 10, 30);
            }
        }
    }
}
