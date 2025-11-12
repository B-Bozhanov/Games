namespace Snake.Tests.SnakeTests
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    using Xunit;

    public class SnakeTests
    {
        [Fact]
        public void NextHeadDirection_Should_Calculated_Correctly() 
        {
            var snake = new Snake(startPossition: 10, length: 6);
            snake.CurrentDirection = Direction.Right;

            var board = new Coordinates(100, 100);
            var obstacle = new Coordinates(999, 999);

            // Up, Left, Left – подготвяме формата
            snake.Move(Direction.Up);
            snake.Move(Direction.Left);
            snake.Move(Direction.Left);

            // Тук опашката е (10,4), главата е (9,4)
            var tailBefore = snake.GetCurrentTailPossition;
            var nextHead = snake.GetNextHeadPossition(Direction.Down);//(Direction.Down); // или както ти е метода
            // sanity check – наистина стъпваме върху опашката
            Assert.Equal(tailBefore, nextHead);
        }

        [Fact]
        public void Move_Should_Advance_Head_Forward()
        {
            var snake = new Snake();
            var oldHead = snake.HeadPossition;

            snake.Move(Direction.Right);

            var newHead = snake.HeadPossition;

            Assert.Equal(oldHead.Col + 1, newHead.Col);
            Assert.Equal(oldHead.Row, newHead.Row);
        }

        [Fact]
        public void Eat_Should_Grow_Snake_By_One()
        {
            var snake = new Snake();

            var before = snake.Body.Count;
            snake.Eat();
            snake.Move(Direction.Right);
            var after = snake.Body.Count;

            Assert.Equal(before + 1, after);
        }

        [Fact]
        public void Move_Should_Not_Grow_When_Not_Eating()
        {
            var snake = new Snake();

            var before = snake.Body.Count;
            snake.Move(Direction.Right);
            var after = snake.Body.Count;

            Assert.Equal(before, after);
        }

        [Fact]
        public void ChangeDirection_Should_Ignore_Opposite()
        {
            var snake = new Snake();
            snake.CurrentDirection = Direction.Right;

            snake.Move(Direction.Left);

            Assert.Equal(Direction.Right, snake.CurrentDirection);
        }

        [Fact]
        public void WillDie_Should_ReturnTrue_When_HitObstacle()
        {
            var snake = new Snake();
            var nextHead = snake.GetNextHeadPossition(Direction.Right);
            var obstacle = nextHead; // на следващата клетка
            var board = new Coordinates(100, 100);

            Assert.True(snake.WillDie(board, obstacle, Direction.Right));
        }

        [Fact]
        public void WillDie_Should_ReturnTrue_When_OutOfBoard()
        {
            var snake = new Snake();
            var smallBoard = new Coordinates(1, 1); // прекалено малка

            Assert.True(snake.WillDie(smallBoard, new Coordinates(500, 500), Direction.Right));
        }

        [Fact]
        public void WillDie_ShouldBeFalse_For_RightDownDownLeftUp()
        {
            var snake = new Snake(10, 6);
            snake.CurrentDirection = Direction.Right;

            var board = new Coordinates(100, 100);
            var obstacle = new Coordinates(999, 999);

            snake.Move(Direction.Down);
            snake.Move(Direction.Down);
            snake.Move(Direction.Left);
            snake.Move(Direction.Up);
            snake.Move(Direction.Up);

            Assert.False(snake.WillDie(board, obstacle, Direction.Up));
        }

        [Fact]
        public void WillDie_ShouldBeTrue_For_RightDownLeftUp()
        {
            var snake = new Snake(10, 6);
            snake.CurrentDirection = Direction.Right;

            var board = new Coordinates(100, 100);
            var obstacle = new Coordinates(999, 999);

            snake.Move(Direction.Down);
            snake.Move(Direction.Left);
            snake.Move(Direction.Up);

            Assert.True(snake.WillDie(board, obstacle, Direction.None));
        }

        [Fact]
        public void Snake_ShouldSurvive_WhenStepsOnTailThatIsAboutToMove()
        {
            // Arrange
            var snake = new Snake(10, 6);
            snake.CurrentDirection = Direction.Right;

            var board = new Coordinates(100, 100);
            var obstacle = new Coordinates(999, 999);

            // Up, Left, Left – подготвяме формата
            snake.Move(Direction.Up);
            snake.Move(Direction.Left);
            snake.Move(Direction.Left);

            // Тук опашката е (10,4), главата е (9,4)
            var tailBefore = snake.GetCurrentTailPossition;
            var nextHead = snake.GetNextHeadPossition(Direction.Down);//(Direction.Down); // или както ти е метода

            // sanity check – наистина стъпваме върху опашката
            Assert.Equal(tailBefore, nextHead);

            // Act – проверяваме WillDie за този ход
            var willDie = snake.WillDie(board, obstacle, Direction.Down /*, Direction.Down ако вече подаваш посока */);

            // Assert
            Assert.False(willDie);
        }
    }
}
