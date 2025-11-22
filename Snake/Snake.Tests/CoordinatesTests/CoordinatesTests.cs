namespace Snake.Tests.CoordinatesTests
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    using Xunit;

    using static SnakeGame.Common.GlobalConstants.GameConstants;

    public class CoordinatesTests
    {
        [Fact]
        public void Move_Up_Should_Decrease_Row()
        {
            var start = new Coordinates(5, 10);

            var result = start.Move(SnakeGame.GameObjects.Enums.Direction.Up);

            Assert.Equal(new Coordinates(4, 10), result);
        }

        [Fact]
        public void Move_Down_Should_Increase_Row()
        {
            var start = new Coordinates(5, 10);

            var result = start.Move(SnakeGame.GameObjects.Enums.Direction.Down);

            Assert.Equal(new Coordinates(6, 10), result);
        }

        [Fact]
        public void Move_Left_Should_Decrease_Col()
        {
            var start = new Coordinates(5, 10);

            var result = start.Move(SnakeGame.GameObjects.Enums.Direction.Left);

            Assert.Equal(new Coordinates(5, 9), result);
        }

        [Fact]
        public void Move_Right_Should_Increase_Col()
        {
            var start = new Coordinates(5, 10);

            var result = start.Move(SnakeGame.GameObjects.Enums.Direction.Right);

            Assert.Equal(new Coordinates(5, 11), result);
        }

        [Fact]
        public void Add_Operator_Should_Sum_Row_And_Col()
        {
            var a = new Coordinates(2, 3);
            var b = new Coordinates(5, 7);

            var result = a + b;

            Assert.Equal(new Coordinates(7, 10), result);
        }

        [Fact]
        public void Subtract_Operator_Should_Subtract_Row_And_Col()
        {
            var a = new Coordinates(7, 10);
            var b = new Coordinates(2, 3);

            var result = a - b;

            Assert.Equal(new Coordinates(5, 7), result);
        }

        [Theory]
        [InlineData(9, 9, 10, 10, true)] // долу вдясно 
        [InlineData(10, 10, 10, 10, false)] // долу вдясно 
        [InlineData(-1, 0, 10, 10, false)]
        [InlineData(0, -1, 10, 10, false)]
        [InlineData(11, 10, 10, 10, false)]
        [InlineData(10, 11, 10, 10, false)]
        public void IsInRange_Should_Return_Correct_Result(int row, int col, int height, int width, bool expected)
        {
            var coord = new Coordinates(row, col);

            var inRange = coord.IsInRange(height, width);

            Assert.Equal(expected, inRange);
        }
    }
}
