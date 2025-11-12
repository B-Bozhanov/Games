namespace SnakeGame.GameObjects.Interfaces
{
    public interface IGameBoard : IBoard
    {
        public void RenderBoard();

        public bool SetSettings();
    }
}
