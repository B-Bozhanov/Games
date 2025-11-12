namespace SnakeGame.GameObjects.Interfaces
{
    public interface IGameBoard
    {
        public Coordinates BoardSize { get; }

        public bool SetSettings();

        public IReadOnlyCollection<Coordinates> Walls { get; }

        public void CreateWalls();
    }
}
