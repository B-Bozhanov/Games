namespace SnakeGame.GameObjects;

using SnakeGame.GameObjects.Abstractions.Base;

using static SnakeGame.Common.GlobalConstants;

public sealed class Snake : BaseSnake
{
    public Snake(int startPossition = SnakeConstants.StartPossitionRow, int length = SnakeConstants.DefaultLength)
        :base(startPossition, length)
    {
        this.CurrentDirection = SnakeConstants.DefaultSnakeDirection;
        for (int i = 0; i < this.body.Count - 2; i++)
        {
            this.tailDirection.Enqueue(this.CurrentDirection);
        }
    }

    protected override void InitialBody(int startPossitionRow, int length)
    {
        for (int col = 1; col <= length; col++)
        {
            this.body.Enqueue(new Coordinates(startPossitionRow, col));
        }
    }
}