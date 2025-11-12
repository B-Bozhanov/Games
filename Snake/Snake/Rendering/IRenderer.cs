namespace SnakeGame.Rendering
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public interface IRenderer
    {
        void ClearElement(Coordinates element);

        void ClearAll();

        public void Draw(Coordinates possition, string symbol, Color color = Color.None);

        public void Draw(CellType[,] Matrix);

        public void Draw(IReadOnlyCollection<Coordinates> coordinates);

        public void Draw(IReadOnlyCollection<Coordinates> coordinates, Color color = Color.None);
    }
}
