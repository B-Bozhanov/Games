namespace SnakeGame.Rendering
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public interface IRenderer
    {
        public void Draw(CellType[,] Matrix);

        public void Draw(CellType[,] prev, CellType[,] curr);
    }
}
