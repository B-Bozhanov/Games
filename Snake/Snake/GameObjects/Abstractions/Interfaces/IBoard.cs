namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface IBoard
    {
        public void Add(Coordinates coordinates, CellType cellType);

        public void Add(IReadOnlyCollection<Coordinates> coordinates, CellType cellType = CellType.None);

        public CellType GetCellType(Coordinates coordinates);

        public void RemoveCellType(Coordinates coordinates);

        public void RemoveAll();

        public IBoardConfig BoardConfig { get; }
    }
}
