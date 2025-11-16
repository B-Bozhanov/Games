namespace SnakeGame.GameObjects
{
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public sealed class ConsoleTheme : ITheme<char, ConsoleColor>
    {
        private readonly char wallsLeftAndRight = '║';
        private readonly char wallsTopAndBottom = '═';
        private readonly char wallTopLeft = '╔';
        private readonly char wallTopRight = '╗';
        private readonly char wallBottomRight = '╝';
        private readonly char wallBottomLeft = '╚';
        private readonly char foodSymbol = '★';
        private readonly char obstacleSymbol = '=';
        private readonly char wallMiddleLeft = '╠';
        private readonly char wallMiddleRight = '╣';
        private readonly char snakeHeadUp = '▲';
        private readonly char snakeHeadDown = '▼';
        private readonly char snakeHeadLeft = '◄';
        private readonly char snakeHeadRight = '►';
        private readonly char snakeBody = '●';
        private readonly char snakeTailUp = '▼';
        private readonly char snakeTailDown = '▲';
        private readonly char snakeTailLeft = '►';
        private readonly char snakeTailRight = '◄';

        public ConsoleColor MapColor(CellType cellType) => cellType switch
        {
            CellType.SnakeHeadUp 
            or CellType.SnakeHeadLeft
            or CellType.SnakeHeadRight
            or CellType.SnakeHeadDown => ConsoleColor.Red,
            CellType.SnakeTailDown
            or CellType.SnakeTailRight
            or CellType.SnakeTailUp
            or CellType.SnakeTailLeft => ConsoleColor.Blue,
            CellType.SnakeBody => ConsoleColor.DarkYellow,
            CellType.SnakeEnemyBody => ConsoleColor.DarkCyan,
            CellType.Food => ConsoleColor.Green,
            CellType.Obstacle => ConsoleColor.Cyan,
            _ => ConsoleColor.Gray
        };

        public char MapSymbol(CellType cellType) => cellType switch
        {
            CellType.WallTopLeft => this.wallTopLeft,
            CellType.WallTopRight => this.wallTopRight,
            CellType.WallBottomLeft => this.wallBottomLeft,
            CellType.WallBottomRight => this.wallBottomRight,
            CellType.WallsTopAndBottom => this.wallsTopAndBottom,
            CellType.WallsLeftAndRight => this.wallsLeftAndRight,
            CellType.Obstacle => this.obstacleSymbol,
            CellType.Food => this.foodSymbol,
            CellType.WallMiddleLeft => this.wallMiddleLeft,
            CellType.WallMiddleRight => this.wallMiddleRight,
            CellType.SnakeHeadUp => this.snakeHeadUp,
            CellType.SnakeHeadDown => this.snakeHeadDown,
            CellType.SnakeHeadLeft => this.snakeHeadLeft,
            CellType.SnakeHeadRight => this.snakeHeadRight,
            CellType.SnakeBody => this.snakeBody,
            CellType.SnakeTailUp => this.snakeTailUp,
            CellType.SnakeTailDown => this.snakeTailDown,
            CellType.SnakeTailLeft => this.snakeTailLeft,
            CellType.SnakeTailRight => this.snakeTailRight,
            CellType.SnakeEnemyBody => this.snakeBody,
            _ => ' ',
        };
    }
}
