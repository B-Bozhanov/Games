namespace SnakeGame.GameObjects.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface ISnake
    {
        public IReadOnlyCollection<Coordinates> Body { get; }

        public bool ShouldEat {  get; }

        public Direction CurrentDirection { get; }

        public Coordinates GetCurrentTailPossition { get; }

        public Coordinates GetLastTailPossition { get; }

        public Coordinates HeadPossition { get; }

        public void Eat();

        public Coordinates GetNextHeadPossition(Direction direction);

        public void Move(Direction direction);

        public bool WillDie(Coordinates boardSize, Coordinates obstacle, Direction direction);
    }
}
