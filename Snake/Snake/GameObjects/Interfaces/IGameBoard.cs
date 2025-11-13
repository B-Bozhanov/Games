namespace SnakeGame.GameObjects.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface IGameBoard : IBoard
    {
        public void CreateBoard();

        public CellType[,] GetMatrix {  get; }

        public bool SetSettings();

        public IReadOnlyCollection<Coordinates> Coordinates { get; set; }
    }
}
