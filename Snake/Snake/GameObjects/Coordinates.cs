namespace SnakeGame.GameObjects;

using SnakeGame.GameObjects.Enums;

public record struct Coordinates(int Row = 0, int Col = 0)
{
    public static readonly Coordinates Up = new(-1, 0);

    public static readonly Coordinates Down = new(1, 0);

    public static readonly Coordinates Left = new(0, -1);

    public static readonly Coordinates Right = new(0, 1);

    public readonly Coordinates Move(Direction direction) => direction switch
    {
        Direction.Up => this + Up,
        Direction.Down => this + Down,
        Direction.Left => this + Left,
        Direction.Right => this + Right,
        _ => this
    };

    public static Coordinates operator + (Coordinates x, Coordinates y) => new(x.Row + y.Row, x.Col + y.Col);

    public static Coordinates operator - (Coordinates a, Coordinates b) => new(a.Row - b.Row, a.Col - b.Col);

    public readonly bool IsInRange(int width, int height)
        => this.Row >= 0 && this.Row < width && this.Col >= 0 && this.Col < height;
}
