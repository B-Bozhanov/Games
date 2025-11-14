namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface IGameBoard : IBoard
    {
        public void CreateBoard();

        public CellType[,] GetBoard {  get; }

        public bool SetSettings();
    }
}
