namespace SnakeGame.Services;

using System.Collections.Generic;

using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Enums;
using SnakeGame.Input.Enums;

public static class DirectionService
{
    public static Direction Get(Coordinates offset) => offset switch
    {
        (-1, 0) => Direction.Up,
        (1, 0) => Direction.Down,
        (0, -1) => Direction.Left,
        (0, 1) => Direction.Right,
        _ => Direction.Right, // fallback
    };

    public static Direction Get(Direction direction) => direction switch
    {
        Direction.Up => Direction.Up,
        Direction.Down => Direction.Down,
        Direction.Left => Direction.Left,
        Direction.Right => Direction.Right,
        _=> Direction.None
    };

    public static IEnumerable<Direction> GetAllListed()
        => new List<Direction>
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left,
        };

    public static IEnumerable<Direction> GetAll()
    {
        yield return Direction.Up;
        yield return Direction.Right;
        yield return Direction.Down;
        yield return Direction.Left;
    }

    public static Coordinates GetOffset(Direction direction) => direction switch
    {
        Direction.Up => new Coordinates(-1, 0),
        Direction.Down => new Coordinates(1, 0),
        Direction.Left => new Coordinates(0, -1),
        Direction.Right => new Coordinates(0, 1),
        _ => throw new InvalidOperationException("Invalid direcion!")
    };

    public static Direction GetByPressedKey(KeyPressed key) => key switch
    {
        KeyPressed.Left => Direction.Left,
        KeyPressed.Right => Direction.Right,
        KeyPressed.Up => Direction.Up,
        KeyPressed.Down => Direction.Down,
        _=> Direction.None
    };

    public static bool IsOppositeDirection(Direction direction, Direction newDirection) =>
        (direction == Direction.Up && newDirection == Direction.Down) ||
        (direction == Direction.Down && newDirection == Direction.Up) ||
        (direction == Direction.Left && newDirection == Direction.Right) ||
        (direction == Direction.Right && newDirection == Direction.Left);
}
