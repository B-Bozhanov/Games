namespace SnakeGame.GameObjects
{
    using System;
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Interfaces;

    using SnakeGame.Common;

    public class ConsoleGameBoard : IGameBoard
    {
        private readonly int gameWidth = GlobalConstants.GameConstants.GameWidth;
        private readonly int gameHeight = GlobalConstants.GameConstants.GameHeight;

        private readonly List<Coordinates> walls;

        private readonly char leftAndRightWallsSymbol = '║';
        private readonly char upAndDownWallsSymbol = '═';
        private readonly char upLeftCorner = '╔';
        private readonly char upRightCorner = '╗';
        private readonly char downRightCorner = '╝';
        private readonly char downLeftCorner = '╚';

        public ConsoleGameBoard()
        {
            this.HeaderHeight = 3;
            var totalRows = this.HeaderHeight + this.gameHeight + 2;
            var totalCols = this.gameWidth + 2;

            this.BoardSize = new Coordinates(totalRows, totalCols);

            this.walls = new List<Coordinates>();
            this.SetSettings();
        }

        public void Score (int score)
        {
            Console.SetCursorPosition(1, 1);
            Console.WriteLine($"Score == {score}");
        }

        private int TopWallRow => this.HeaderHeight;                     // ред на горната стена
        private int BottomWallRow => this.HeaderHeight + this.gameHeight + 1; // ред на долната стена

        private int LeftWallCol => 0;                 // лява стена
        private int RightWallCol => this.gameWidth + 1; // дясна стена

        public Coordinates BoardSize { get; }

        public int HeaderHeight { get; }

        public IReadOnlyCollection<Coordinates> Walls => this.walls;

        public void CreateWalls()
        {
            this.walls.Clear();

            this.CreateLeftRightSide();
            this.CreateUpDownSide();

            foreach (var item in this.walls)
            {
                Console.SetCursorPosition(item.Col, item.Row);
                Console.Write(this.GetBorderSymbol(item));
            }
        }

        public bool SetSettings()
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int totalRows = this.BoardSize.Row;
            int totalCols = this.BoardSize.Col;

            totalCols = Math.Min(totalCols, Console.LargestWindowWidth);
            totalRows = Math.Min(totalRows, Console.LargestWindowHeight);

            // 1) Ако трябва да УВЕЛИЧАВАМЕ – първо увеличаваме буфера
            if (totalCols > Console.BufferWidth || totalRows > Console.BufferHeight)
            {
                Console.SetBufferSize(
                    Math.Max(totalCols, Console.BufferWidth),
                    Math.Max(totalRows, Console.BufferHeight));
            }

            // 2) Свиваме прозореца до желаните размери (винаги <= buffer)
            Console.SetWindowSize(totalCols, totalRows);

            // 3) По желание: свиваме буфера до прозореца (сега е легално)
            Console.SetBufferSize(totalCols, totalRows);

            Console.Title = "SnakeGame";
            return true;
        }

        private void CreateUpDownSide()
        {
            int topRow = this.TopWallRow;
            int bottomRow = this.BottomWallRow;

            for (int col = this.LeftWallCol; col <= this.RightWallCol; col++)
            {
                this.walls.Add(new Coordinates(topRow, col));
                this.walls.Add(new Coordinates(bottomRow, col));
            }
        }

        /// <summary>
        /// Лява и дясна вертикална стена – от горната до долната.
        /// </summary>
        private void CreateLeftRightSide()
        {
            int firstCol = this.LeftWallCol;
            int lastCol = this.RightWallCol;
            int topRow = this.TopWallRow;
            int bottomRow = this.BottomWallRow;

            for (int row = topRow; row <= bottomRow; row++)
            {
                this.walls.Add(new Coordinates(row, firstCol));
                this.walls.Add(new Coordinates(row, lastCol));
            }
        }

        /// <summary>
        /// Избира правилния символ за рамката според това къде се намира клетката.
        /// </summary>
        private char GetBorderSymbol(Coordinates coordinates)
        {
            bool isTop = coordinates.Row == this.TopWallRow;
            bool isBottom = coordinates.Row == this.BottomWallRow;
            bool isLeft = coordinates.Col == this.LeftWallCol;
            bool isRight = coordinates.Col == this.RightWallCol;

            if (isTop && isLeft)
            {
                return this.upLeftCorner;
            }

            if (isTop && isRight)
            {
                return this.upRightCorner;
            }

            if (isBottom && isLeft)
            {
                return this.downLeftCorner;
            }

            if (isBottom && isRight)
            {
                return this.downRightCorner;
            }

            if (isTop || isBottom)
            {
                return this.upAndDownWallsSymbol;
            }

            return this.leftAndRightWallsSymbol;
        }
    }
}
