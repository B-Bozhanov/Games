namespace SnakeGame.GameObjects;

using SnakeGame.GameObjects.Enums;

public sealed class Snake
{
    private bool shouldEat;
    private readonly Queue<Coordinates> body = new();

    public Snake(int startPossitionRow = 0, int length = 6)
    {
        this.shouldEat = false;
        this.InitialBody(startPossitionRow, length);
        this.CurrentDirection = Direction.Right; //Default direction
    }

    public IReadOnlyCollection<Coordinates> Body => this.body;

    public Direction CurrentDirection { get; set; }

    public Coordinates NextHeadPossition => this.GetNextHeadPossition();

    public Coordinates HeadPossition => this.GetHeadPossition();

    public Coordinates GetTailPossition => this.body.Peek();

    public void Eat() => this.shouldEat = true;

    public void Move(Direction newDirection)
    {
        this.ChangeDirection(newDirection);

        var nextHead = this.NextHeadPossition;
        this.body.Enqueue(nextHead);

        if (!this.shouldEat)
        {
            this.body.Dequeue();
        }

        this.shouldEat = false;
    }

    public bool WillDie(Coordinates board, Coordinates obstacle)
        =>      this.WillCollideWithSelf()
             || this.WillHitObstacle(obstacle)
             || !this.NextHeadPossition.IsInRange(board.Row, board.Col);
   
    private void ChangeDirection(Direction newDirection)
    {
        if (IsOppositeDirection(newDirection))
        {
            return;
        }

        if (newDirection != Direction.None)
        {
            this.CurrentDirection = newDirection;
        }
    }

    private Coordinates GetNextHeadPossition()
    {
        var currentHeadPossition = this.body.Last();
        var nextHeadPossition = currentHeadPossition.Move(this.CurrentDirection);
        return nextHeadPossition;
    }

    private Coordinates GetHeadPossition()
    {
        return this.body.Last();
    }

    private void InitialBody(int startPossitionRow, int length)
    {
        for (int i = 0; i < length; i++)
        {
            this.body.Enqueue(new Coordinates(startPossitionRow, i));
        }
    }

    private bool IsOppositeDirection(Direction newDirection) =>
        (this.CurrentDirection == Direction.Up && newDirection == Direction.Down) ||
        (this.CurrentDirection == Direction.Down && newDirection == Direction.Up) ||
        (this.CurrentDirection == Direction.Left && newDirection == Direction.Right) ||
        (this.CurrentDirection == Direction.Right && newDirection == Direction.Left);

    private bool WillCollideWithSelf() 
        => this.body.Contains(this.NextHeadPossition);

    private bool WillHitObstacle(Coordinates obstacle)
        => this.NextHeadPossition == obstacle;
}
