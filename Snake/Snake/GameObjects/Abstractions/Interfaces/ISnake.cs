namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface ISnake
    {
        public IReadOnlyCollection<Coordinates> Body { get; }

        public bool ShouldEat { get; }

        public Direction CurrentDirection { get; }

        public Coordinates GetCurrentTailPossition { get; }

        public Coordinates GetLastTailPossition { get; }

        public CellType NextHeadPossitionSymbol { get; }

        public CellType BodySymbol { get; }

        public CellType NextTailPossitionSymbol { get; }

        public Coordinates HeadPossition { get; }

        public void Eat();

        public Coordinates GetNextHeadPossition(Direction direction);

        public void Move(Direction direction);

        public bool WillCollideWithSelf(Coordinates nextHead);
    }
}