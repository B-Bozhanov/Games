namespace SnakeGame.GameObjects.Abstractions.Base
{
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    using static SnakeGame.Common.GlobalConstants;

    public abstract class BaseSnake : ISnake
    {
        private bool shouldEat;
        protected readonly Queue<Coordinates> body = new();
        protected readonly Queue<Direction> tailDirection = new();

        public BaseSnake(int startPossition, int length)
        {
            this.shouldEat = false;
            this.InitialBody(startPossition, length);
           // this.CurrentDirection = SnakeConstants.DefaultSnakeDirection;
            this.BodySymbol = CellType.SnakeBody;
        }

        public IReadOnlyCollection<Coordinates> Body => this.body;

        public virtual Direction CurrentDirection { get; set; }

        public CellType NextHeadPossitionSymbol { get; private set; }

        public CellType BodySymbol { get; }

        public CellType NextTailPossitionSymbol { get; private set; }

        public Coordinates GetCurrentTailPossition => this.body.Peek();

        public Coordinates GetLastTailPossition { get; private set; }

        public Coordinates HeadPossition => this.GetHeadPossition();

        public bool ShouldEat => this.shouldEat;

        public void Eat() => this.shouldEat = true;

        public Coordinates GetNextHeadPossition(Direction direction)
        {
            var currentDirection = this.ChangeDirection(direction);
            var nextHeadPossition = this.HeadPossition.Move(currentDirection);
            this.SetHeadTailSymbol(currentDirection);
            return nextHeadPossition;
        }

        public void Move(Direction newDirection)
        {
            var currentDirection = this.ChangeDirection(newDirection);
            this.CurrentDirection = currentDirection;

            var nextHead = this.GetNextHeadPossition(currentDirection);
            this.body.Enqueue(nextHead);
            this.tailDirection.Enqueue(currentDirection);

            if (!this.shouldEat)
            {
                this.GetLastTailPossition = this.body.Peek();
                this.body.Dequeue();
                this.tailDirection.Dequeue();
            }

            this.shouldEat = false;
        }

        private Direction ChangeDirection(Direction newDirection)
        {
            if (newDirection == Direction.None || IsOppositeDirection(newDirection))
            {
                return this.CurrentDirection;
            }

            return newDirection;
        }

        private void SetHeadTailSymbol(Direction newDirection)
        {
            this.NextHeadPossitionSymbol = newDirection switch
            {
                Direction.Up => CellType.SnakeHeadUp,
                Direction.Down => CellType.SnakeHeadDown,
                Direction.Left => CellType.SnakeHeadLeft,
                Direction.Right => CellType.SnakeHeadRight,
                _ => throw new NotSupportedException()
            };


            var tailDirection = this.tailDirection.Peek();
            this.NextTailPossitionSymbol = tailDirection switch
            {
                Direction.Up => CellType.SnakeTailUp,
                Direction.Down => CellType.SnakeTailDown,
                Direction.Left => CellType.SnakeTailLeft,
                Direction.Right => CellType.SnakeTailRight,
                _ => throw new NotSupportedException()
            };
        }

        private Coordinates GetHeadPossition()
        {
            return this.body.Last();
        }

        protected virtual void InitialBody(int startPossitionRow, int length)
        {
           
        }

        private bool IsOppositeDirection(Direction newDirection) =>
            (this.CurrentDirection == Direction.Up && newDirection == Direction.Down) ||
            (this.CurrentDirection == Direction.Down && newDirection == Direction.Up) ||
            (this.CurrentDirection == Direction.Left && newDirection == Direction.Right) ||
            (this.CurrentDirection == Direction.Right && newDirection == Direction.Left);

        public bool WillCollideWithSelf(Coordinates nextHead)
        {
            if (!this.shouldEat && nextHead == this.GetCurrentTailPossition)
            {
                return false;
            }

            return this.body.Contains(nextHead);
        }
    }
}
