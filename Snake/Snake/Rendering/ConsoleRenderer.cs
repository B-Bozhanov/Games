namespace SnakeGame.Rendering
{
    using System.Collections.Generic;

    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public class ConsoleRenderer : IRenderer
    {
        private readonly char wallsLeftAndRight = '║';
        private readonly char wallsTopAndBottom = '═';
        private readonly char wallTopLeft = '╔';
        private readonly char wallTopRight = '╗';
        private readonly char wallBottomRight = '╝';
        private readonly char wallBottomLeft = '╚';
        private readonly char snakeBody = '*';
        private readonly char foodSymbol = '@';

        public void ClearAll() => Console.Clear();

        public void ClearElement(Coordinates element)
        {
            Console.SetCursorPosition(element.Col, element.Row);
            Console.Write(' ');
        }

        public char Map(CellType cell) => cell switch
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

        public void Draw(Coordinates possition, string symbol, Color color = Color.None)
        {
            Console.SetCursorPosition(possition.Col, possition.Row);
            Console.Write(symbol);
        }

        public void Draw(IReadOnlyCollection<Coordinates> coordinates)
        {
            foreach (var item in coordinates)
            {
                Console.SetCursorPosition(item.Col, item.Row);
                Console.Write('*');
            }
        }

        public void Draw(IReadOnlyCollection<Coordinates> coordinates, Color color = Color.None)
        {
            throw new NotImplementedException();
        }

        public void Draw(CellType[,] matrix)
        {
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    var cell = matrix[row, col];

                    var symbol = this.Map(cell);

                    Console.SetCursorPosition(col, row);
                    Console.Write(symbol);
                }
            }
        }
    }
}
