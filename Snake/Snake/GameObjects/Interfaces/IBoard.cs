namespace SnakeGame.GameObjects.Interfaces
{
    public interface IBoard
    {
        public Coordinates BoardSize { get; }

        public IReadOnlyCollection<Coordinates> Walls { get; }
    }
}
