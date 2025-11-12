namespace SnakeGame.Rendering
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public interface IRenderer
    {
        void ClearElement(Coordinates element);

        void ClearAll();

        public void Drow(Coordinates possition, char symbol, Color color = Color.None);

        public void Drow(IReadOnlyCollection<Coordinates> coordinates);

        public void Drow(IReadOnlyCollection<Coordinates> coordinates, Color color = Color.None);
    }
}
