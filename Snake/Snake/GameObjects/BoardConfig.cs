namespace SnakeGame.GameObjects
{
    using SnakeGame.GameObjects.Abstractions.Interfaces;

    public class BoardConfig(
        int playableWidth,
        int playableHeight,
        int headerHeight,
        int wallsWidth) : IBoardConfig
    {
        public int PlayableWidth { get; } = playableWidth;

        public int PlayableHeight { get; } = playableHeight;

        public int HeaderHeight { get; } = headerHeight;

        public int WallsWidth { get; } = wallsWidth;

        public int PlayableStartRow => this.HeaderHeight + this.WallsWidth;

        public int PlayableEndRow => this.PlayableHeight - this.WallsWidth - this.WallsWidth;

        public int PlayableStartCol => this.WallsWidth;

        public int PlayableEndCol => this.PlayableWidth - this.WallsWidth - this.WallsWidth;

        public int TotalRows => HeaderHeight + WallsWidth + PlayableHeight + WallsWidth;

        public int TotalCols => this.PlayableWidth + this.WallsWidth + this.WallsWidth;
    }
}
