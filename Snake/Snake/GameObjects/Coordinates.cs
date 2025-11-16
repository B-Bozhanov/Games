namespace SnakeGame.GameObjects;

using SnakeGame.GameObjects.Enums;
using SnakeGame.Common;

public record struct Coordinates(int Row = 0, int Col = 0)
{
    private static readonly Coordinates Up = new(-1, 0);

    private static readonly Coordinates Down = new(1, 0);

    private static readonly Coordinates Left = new(0, -1);

    private static readonly Coordinates Right = new(0, 1);

    public readonly Coordinates Move(Direction direction) => direction switch
    {
        Direction.Up => this + Up,
        Direction.Down => this + Down,
        Direction.Left => this + Left,
        Direction.Right => this + Right,
        _ => this
    };

    public static Coordinates operator +(Coordinates x, Coordinates y) => new(x.Row + y.Row, x.Col + y.Col);

    public static Coordinates operator -(Coordinates a, Coordinates b) => new(a.Row - b.Row, a.Col - b.Col);

    public readonly bool IsInRange(int height, int width) =>
           this.Row > GlobalConstants.GameConstants.HeaderHeight
        && this.Row < height -1
        && this.Col > 0
        && this.Col < width -1;
}
