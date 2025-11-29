namespace SnakeGame.GameObjects;

using SnakeGame.Common;
using SnakeGame.GameObjects.Enums;
using SnakeGame.Services;

public record struct Coordinates(int Row = 0, int Col = 0)
{
    public readonly Coordinates Move(Direction direction) =>
        this + DirectionService.GetOffset(direction);

    public static Coordinates operator +(Coordinates x, Coordinates y) => new(x.Row + y.Row, x.Col + y.Col);

    public static Coordinates operator -(Coordinates a, Coordinates b) => new(a.Row - b.Row, a.Col - b.Col);

    public readonly bool IsInRange(int height, int width) =>
           this.Row > GlobalConstants.GameConstants.HeaderHeight
        && this.Row < height - 1
        && this.Col > 0
        && this.Col < width - 1;
}