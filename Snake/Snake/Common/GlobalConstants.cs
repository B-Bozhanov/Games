namespace SnakeGame.Common
{
    using SnakeGame.GameObjects.Enums;

    public static class GlobalConstants
    {
        public static class GameConstants
        {
            public const int GameDefaultFps = 8;
            public const int PlayableBoardWidth = 120;
            public const int PlayableBoardHeight = 25;
            public const int HeaderHeight = 3;
            public const int WallsWidth = 1;
        }

        public static class SnakeConstants
        {
            public const int DefaultLength = 6;
            public const int StartPossitionRow = GameConstants.HeaderHeight + GameConstants.WallsWidth;
            public const string GameName = "Snake";

            public const Direction DefaultSnakeDirection = Direction.Right;
            public const Direction DefaultSnakeEnemyDirection = Direction.Left;
        }
    }
}
