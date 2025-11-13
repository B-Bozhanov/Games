namespace SnakeGame.Rendering
{
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.GameObjects.Interfaces;

    public class ConsoleRenderer(ITheme<char> theme) : IRenderer
    {
        private readonly ITheme<char> theme = theme;

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
                    var symbol = this.theme.Map(cell);

                    Console.SetCursorPosition(c, r);
                    Console.Write(symbol);
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

                    var symbol = this.theme.Map(cell);

                    Console.SetCursorPosition(col, row);
                    Console.Write(symbol);
                }
            }
        }
    }
}
