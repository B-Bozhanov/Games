namespace SnakeGame.Rendering
{
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public class ConsoleRenderer(ITheme<char, ConsoleColor> theme) : IRenderer
    {
        private readonly ITheme<char, ConsoleColor> theme = theme;

        public void Draw(CellType[,] prev, CellType[,] curr)
        {
            var rows = curr.GetLength(0);
            var cols = curr.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (curr[r, c] == prev[r, c])
                    {
                        continue;
                    }
                    var cell = curr[r, c];
                    var symbol = this.theme.MapSymbol(cell);

                    var symbolColor = this.theme.MapColor(cell);
                    Console.ForegroundColor = symbolColor;
                    Console.SetCursorPosition(c, r);
                    Console.Write(symbol);
                    Console.ResetColor();
                }
            }
        }

        public void Draw(CellType[,] matrix)
        {
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    var cell = matrix[row, col];

                    var symbol = this.theme.MapSymbol(cell);
                    var symbolColor = this.theme.MapColor(cell);
                    Console.ForegroundColor = symbolColor;
                    Console.SetCursorPosition(col, row);
                    Console.Write(symbol);
                    Console.ResetColor();
                }
            }
        }
    }
}
