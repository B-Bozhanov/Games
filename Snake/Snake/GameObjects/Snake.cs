namespace SnakeGame.GameObjects;

using SnakeGame.GameObjects.Abstractions.Interfaces;
using SnakeGame.GameObjects.Enums;

using static SnakeGame.Common.GlobalConstants;

public sealed class Snake : ISnake
{
    private bool shouldEat;
    private readonly Queue<Coordinates> body = new();

    public Snake(int startPossition = SnakeConstants.StartPossitionRow, int length = SnakeConstants.DefaultLength)
    {
        this.shouldEat = false;
        this.InitialBody(startPossition, length);
        this.CurrentDirection = SnakeConstants.DefaultDirection;
    }

    public IReadOnlyCollection<Coordinates> Body => this.body;

    public Direction CurrentDirection { get; set; }

    public Coordinates GetCurrentTailPossition => this.body.Peek();

    public Coordinates GetLastTailPossition { get; private set; }

    public Coordinates HeadPossition => this.GetHeadPossition();

    public bool ShouldEat => this.shouldEat;

    public void Eat() => this.shouldEat = true;

    public Coordinates GetNextHeadPossition(Direction direction)
    {
        var currentDirection = this.ChangeDirection(direction);
        var nextHeadPossition = this.HeadPossition.Move(currentDirection);
        return nextHeadPossition;
    }

    public void Move(Direction newDirection)
    {
        var cuurentDirection = this.ChangeDirection(newDirection);
        this.CurrentDirection = cuurentDirection;

        var nextHead = this.GetNextHeadPossition(cuurentDirection);
        this.body.Enqueue(nextHead);

        if (!this.shouldEat)
        {
            this.GetLastTailPossition = this.body.Peek();
            this.body.Dequeue();
        }

        this.shouldEat = false;
    }

    public bool WillDie(IBoardConfig boardConfig, Coordinates obstacle, Direction direction)
             => this.WillCollideWithSelf(this.GetNextHeadPossition(direction))
             || this.WillHitObstacle(direction, obstacle)
             || !this.GetNextHeadPossition(direction).IsInRange(boardConfig.PlayableHeight, boardConfig.PlayableWidth);

    private Direction ChangeDirection(Direction newDirection)
    {
        if (newDirection == Direction.None || IsOppositeDirection(newDirection))
        {
            return this.CurrentDirection; ;
        }

        return newDirection;
    }

    private Coordinates GetHeadPossition()
    {
        return this.body.Last();
    }

    private void InitialBody(int startPossitionRow, int length)
    {
        for (int row = 1; row <= length; row++)
        {
            this.body.Enqueue(new Coordinates(startPossitionRow, row));
        }
    }

    private bool IsOppositeDirection(Direction newDirection) =>
        (this.CurrentDirection == Direction.Up && newDirection == Direction.Down) ||
        (this.CurrentDirection == Direction.Down && newDirection == Direction.Up) ||
        (this.CurrentDirection == Direction.Left && newDirection == Direction.Right) ||
        (this.CurrentDirection == Direction.Right && newDirection == Direction.Left);

    private bool WillCollideWithSelf(Coordinates nextHead)
    {
        if (!this.shouldEat && nextHead == this.GetCurrentTailPossition)
        {
            return false;
        }

        return this.body.Contains(nextHead);
    }

    private bool WillHitObstacle(Direction direction, Coordinates obstacle)
        => this.GetNextHeadPossition(direction) == obstacle;
}
