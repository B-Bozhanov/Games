namespace SnakeGame.Input;

using SnakeGame.Input.Enums;

public sealed class ConsoleInputReader : IInputReader
{
    public KeyPressed GetInput()
    {
        if (!Console.KeyAvailable)
        {
            return KeyPressed.None;
        }

        ConsoleKey key = Console.ReadKey(intercept: true).Key;

        return key switch
        {
            ConsoleKey.LeftArrow => KeyPressed.Left,
            ConsoleKey.RightArrow => KeyPressed.Right,
            ConsoleKey.UpArrow => KeyPressed.Up,
            ConsoleKey.DownArrow => KeyPressed.Down,
            ConsoleKey.W => KeyPressed.Up,
            ConsoleKey.S => KeyPressed.Down,
            ConsoleKey.A => KeyPressed.Left,
            ConsoleKey.D => KeyPressed.Right,
            _ => KeyPressed.None,
        };
    }
}