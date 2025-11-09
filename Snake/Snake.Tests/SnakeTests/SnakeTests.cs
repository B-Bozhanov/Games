namespace Snake.Tests.SnakeTests
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    using Xunit;

    public class SnakeTests
    {
        [Fact]
        public void Move_Should_Advance_Head_Forward()
        {
            var snake = new Snake(startPossitionRow: 0, length: 3);
            var oldHead = snake.HeadPossition;

            snake.Move(Direction.Right);

            var newHead = snake.HeadPossition;

            Assert.Equal(oldHead.Col + 1, newHead.Col);
            Assert.Equal(oldHead.Row, newHead.Row);
        }

        [Fact]
        public void Eat_Should_Grow_Snake_By_One()
        {
            var snake = new Snake(startPossitionRow: 0, length: 3);

            var before = snake.Body.Count;
            snake.Eat();
            snake.Move(Direction.Right);
            var after = snake.Body.Count;

            Assert.Equal(before + 1, after);
        }

        [Fact]
        public void Move_Should_Not_Grow_When_Not_Eating()
        {
            var snake = new Snake(startPossitionRow: 0, length: 3);

            var before = snake.Body.Count;
            snake.Move(Direction.Right);
            var after = snake.Body.Count;

            Assert.Equal(before, after);
        }

        [Fact]
        public void ChangeDirection_Should_Ignore_Opposite()
        {
            var snake = new Snake(0, 3);
            snake.CurrentDirection = Direction.Right;

            snake.Move(Direction.Left);

            Assert.Equal(Direction.Right, snake.CurrentDirection);
        }

        [Fact]
        public void WillDie_Should_ReturnTrue_When_HitObstacle()
        {
            var snake = new Snake(0, 3);
            var obstacle = snake.NextHeadPossition; // на следващата клетка
            var board = new Coordinates(100, 100);

            Assert.True(snake.WillDie(board, obstacle));
        }

        [Fact]
        public void WillDie_Should_ReturnTrue_When_OutOfBoard()
        {
            var snake = new Snake(0, 3);
            var smallBoard = new Coordinates(1, 1); // прекалено малка

            Assert.True(snake.WillDie(smallBoard, new Coordinates(500, 500)));
        }

        [Fact]
        public void WillDie_Should_ReturnTrue_When_CollideWithSelf()
        {
            var snake = new Snake(0, 3);

            // симулираме ситуация - snake върви надясно, после наляво
            snake.Move(Direction.Down);
            snake.Move(Direction.Left);
            snake.Move(Direction.Up);

            var board = new Coordinates(100, 100);
            var obstacle = new Coordinates(999, 999);

            Assert.True(snake.WillDie(board, obstacle));
        }
    }
}
