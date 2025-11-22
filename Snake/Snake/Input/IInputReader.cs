namespace SnakeGame.Input
{
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.Input.Enums;

    public interface IInputReader
    {
        public KeyPressed GetInput();
    }
}
