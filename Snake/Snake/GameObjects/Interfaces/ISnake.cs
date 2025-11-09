namespace SnakeGame.GameObjects.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface ISnake
    {
        public IReadOnlyCollection<Coordinates> Body { get; }

        public Direction CurrentDirection { get; }

        public Coordinates GetTailPossition {  get; }

        public Coordinates NextHeadPossition { get; }

        public Coordinates HeadPossition { get; }

        public void Eat();

        public void Move(Direction direction);

        public bool WillDie(Coordinates boardSize, Coordinates obstacle);
    }
}
