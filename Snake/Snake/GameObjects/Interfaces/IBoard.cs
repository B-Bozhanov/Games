namespace SnakeGame.GameObjects.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface IBoard
    {
        public void Add(Coordinates coordinates, CellType cellType);

        public void Add(IReadOnlyCollection<Coordinates> coordinates, CellType cellType = CellType.None);

        public void RemoveCellType(Coordinates coordinates);

        public void RemoveAll();

        public Coordinates BoardSize { get; }

        public IReadOnlyCollection<Coordinates> Walls { get; }
    }
}
