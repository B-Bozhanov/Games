namespace Snake.Tests.FoodTests;

using SnakeGame.GameObjects;

using Xunit;

public class FoodTests
{
    [Fact]
    public void Constructor_Sets_Properties_Correctly()
    {
        // Arrange
        var coords = new Coordinates(5, 10);
        var lifetime = TimeSpan.FromSeconds(10);

        // Act
        var food = new Food(coords, lifetime);

        // Assert
        Assert.Equal('@', food.Symbol);
        Assert.Equal(coords, food.Coordinates);
        Assert.Equal(lifetime, food.LifeTime);
        Assert.True(food.StartTime <= DateTime.UtcNow);
    }

    [Fact]
    public void IsExpired_Should_BeFalse_Before_Lifetime()
    {
        // Arrange
        var food = new Food(new Coordinates(0, 0), TimeSpan.FromMilliseconds(200));

        // Act
        Thread.Sleep(50); // 50ms < 200ms

        // Assert
        Assert.False(food.IsExpired);
    }

    [Fact]
    public void IsExpired_Should_BeTrue_After_Lifetime()
    {
        // Arrange
        var food = new Food(new Coordinates(0, 0), TimeSpan.FromMilliseconds(100));

        // Act
        Thread.Sleep(150); // 150ms > 100ms

        // Assert
        Assert.True(food.IsExpired);
    }
}
