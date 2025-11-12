namespace SnakeGame.GameObjects.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface IGameBoard : IBoard
    {
        public void CreateBoarder();

        public void RenderBoard();

        public bool SetSettings();

        public IReadOnlyCollection<Coordinates> Coordinates { get; set; }
    }
}
