namespace SnakeGame.Common
{
    using SnakeGame.GameObjects.Enums;

    public static class GlobalConstants
    {
        public static class GameConstants
        {
            public const int GameDefaultFps = 8;
            public const int GameWidth = 120;
            public const int GameHeight = 25;
            public const int HeaderHeight = 3;
            public const int WallsWidth = 1;
        }

        public static class SnakeConstants
        {
            public const int DefaultLength = 6;
            public const int StartPossitionRow = GameConstants.HeaderHeight + GameConstants.WallsWidth;
            public const string GameName = "Snake";

            public const Direction DefaultDirection = Direction.Right;
        }
    }
}
