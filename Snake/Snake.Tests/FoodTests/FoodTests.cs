namespace Snake.Tests.FoodTests;

using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Enums;

using Xunit;

public class FoodTests
{
    private const int HeaderHeight = 3;

    [Fact]
    public void Constructor_Sets_Properties_Correctly()
    {
        // Arrange
        var coords = new Coordinates(5, 10);
        var lifetime = TimeSpan.FromSeconds(10);

        // Act
        var food = new Food(coords, lifetime);

        // Assert
        Assert.Equal(CellType.Food, food.Coordinates.Symbol);
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

    [Fact]
    public void IsExpired_Should_BeFalse_Immediately_After_Creation()
    {
        var food = new Food(new Coordinates(1, 1), TimeSpan.FromSeconds(1));

        Assert.False(food.IsExpired);
    }

    [Fact]
    public void Multiple_Foods_Should_Have_Different_StartTimes()
    {
        var f1 = new Food(new Coordinates(0, 0), TimeSpan.FromSeconds(5));
        Thread.Sleep(10); // малко забавяне
        var f2 = new Food(new Coordinates(0, 1), TimeSpan.FromSeconds(5));

        Assert.True(f2.StartTime >= f1.StartTime);
    }

    [Fact]
    public void GetFood_Should_Return_OnlyPlayableCells_NotOnWalls_NotInHeader_NotOnSnake()
    {
        // Arrange: тотален размер на конзолата (редове, колони)
        var board = new Coordinates(25, 80);

        // Змията – достатъчно дълга, за да има реален шанс за колизия при избор
        var snake = new Snake(10, 6);
        snake.CurrentDirection = SnakeGame.GameObjects.Enums.Direction.Right;
        // малко ходове, за да "разбъркаме" тялото
        snake.Move(SnakeGame.GameObjects.Enums.Direction.Right);
        snake.Move(SnakeGame.GameObjects.Enums.Direction.Down);
        snake.Move(SnakeGame.GameObjects.Enums.Direction.Left);

        var foodFactory = new FoodFactory();

        // Подготвяме стените (рамка): горна/долна на headerHeight и board.Row-1, лява/дясна на 0 и board.Col-1
        var walls = BuildWalls(board, HeaderHeight);
        var snakeCells = new HashSet<Coordinates>(snake.Body);

        // Act + Assert (многократно пробване, тъй като е RNG)
        for (int i = 0; i < 1000000; i++)
        {
            var food = foodFactory.CreateFood(board, snake.Body);
            var c = food.Coordinates;

            // 1) В рамките на борда (дефензивни проверки)
            Assert.InRange(c.Row, 0, board.Row - 1);
            Assert.InRange(c.Col, 0, board.Col - 1);

            // 2) Не в хедъра
            Assert.True(c.Row >= HeaderHeight + 1, $"Food in header: row={c.Row}");

            // 3) Не върху стените (рамката)
            Assert.DoesNotContain(c, walls);

            // 4) В playable интериора: редове (HeaderHeight+1 .. board.Row-2), колони (1 .. board.Col-2)
            Assert.InRange(c.Row, HeaderHeight + 1, board.Row - 2);
            Assert.InRange(c.Col, 1, board.Col - 2);

            // 5) Да не попада върху змията
            Assert.DoesNotContain(c, snakeCells);
        }
    }

    [Theory]
    [InlineData(5, 5)]   // твърде малко за header+рамка+интериор
    [InlineData(4, 80)]  // твърде малко редове (header=3 + top wall + bottom wall)
    [InlineData(25, 2)]  // твърде малко колони (лява+дясна стена)
    public void GetFood_Should_Throw_When_NoPlayableSpace(int rows, int cols)
    {
        var board = new Coordinates(rows, cols);
        var snake = new Snake(10, 6);
        var foodFactory = new FoodFactory();

        // Очакваме фабриката да сигнализира, че няма валидни клетки за храна
        Assert.ThrowsAny<Exception>(() => foodFactory.CreateFood(board, snake.Body));
    }

    private static HashSet<Coordinates> BuildWalls(Coordinates board, int headerHeight)
    {
        var set = new HashSet<Coordinates>();

        int topRow = headerHeight;          // горна стена
        int bottomRow = board.Row - 1;      // долна стена
        int leftCol = 0;                    // лява стена
        int rightCol = board.Col - 1;       // дясна стена

        // Горна/долна хоризонтална стена
        for (int col = leftCol; col <= rightCol; col++)
        {
            set.Add(new Coordinates(topRow, col));
            set.Add(new Coordinates(bottomRow, col));
        }

        // Лява/дясна вертикална стена
        for (int row = topRow; row <= bottomRow; row++)
        {
            set.Add(new Coordinates(row, leftCol));
            set.Add(new Coordinates(row, rightCol));
        }

        return set;
    }
}
