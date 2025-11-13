namespace SnakeGame.GameObjects
{
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.GameObjects.Interfaces;

    public sealed class ConsoleTheme : ITheme<char>
    {
        private readonly char wallsLeftAndRight = '║';
        private readonly char wallsTopAndBottom = '═';
        private readonly char wallTopLeft = '╔';
        private readonly char wallTopRight = '╗';
        private readonly char wallBottomRight = '╝';
        private readonly char wallBottomLeft = '╚';
        private readonly char snakeBody = '*';
        private readonly char foodSymbol = '@';

        public char Map(CellType cellType) => cellType switch
        {
            CellType.WallTopLeft => this.wallTopLeft,
            CellType.WallTopRight => this.wallTopRight,
            CellType.WallBottomLeft => this.wallBottomLeft,
            CellType.WallBottomRight => this.wallBottomRight,
            CellType.WallsTopAndBottom => this.wallsTopAndBottom,
            CellType.WallsLeftAndRight => this.wallsLeftAndRight,
            CellType.SnakeBody => this.snakeBody,
            CellType.Food => this.foodSymbol,
            _ => ' ',
        };
    }
}
