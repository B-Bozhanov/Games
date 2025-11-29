namespace SnakeGame.Input
{
    using SnakeGame.Input.Enums;

    public interface IInputReader
    {
        public KeyPressed GetInput();
    }
}