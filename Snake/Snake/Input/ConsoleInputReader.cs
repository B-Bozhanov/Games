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
            ConsoleKey.W => Direction.Up,
            ConsoleKey.S => Direction.Down,
            ConsoleKey.A => Direction.Left,
            ConsoleKey.D => Direction.Right,
            _=> Direction.None,
        };
    }
}
