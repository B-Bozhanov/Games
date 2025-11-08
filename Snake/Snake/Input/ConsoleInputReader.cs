namespace SnakeGame.Input;

using SnakeGame.GameObjects.Enums;

public sealed class ConsoleInputReader : IInputReader
{
    public Direction GetInput()
    {
        if (!Console.KeyAvailable)
        {
            return Direction.None;
        }

        ConsoleKey key = Console.ReadKey(intercept: true).Key;

        return key switch
        {
            ConsoleKey.LeftArrow => Direction.Left,
            ConsoleKey.RightArrow => Direction.Right,
            ConsoleKey.UpArrow => Direction.Up,
            ConsoleKey.DownArrow => Direction.Down,
            _=> Direction.None,
        };
    }
}
