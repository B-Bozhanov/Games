namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    public interface IBoardConfig
    {
        public int PlayableWidth { get; }

        public int PlayableHeight { get; }

        public int HeaderHeight { get; }

        public int WallsWidth { get; }

        public int PlayableStartRow { get; }

        public int PlayableEndRow { get; }

        public int PlayableStartCol { get; }

        public int PlayableEndCol { get; }

        public int TotalRows { get; }

        public int TotalCols { get; }
    }
}
