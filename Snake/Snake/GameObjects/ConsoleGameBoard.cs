namespace SnakeGame.GameObjects
{
    using System;
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Interfaces;

    using SnakeGame.Common;
    using SnakeGame.Rendering;
    using SnakeGame.GameObjects.Enums;

    public class ConsoleGameBoard : BaseBoard
    {
        private readonly char wallsLeftAndRight = '║';
        private readonly char wallsTopAndBottom = '═';
        private readonly char wallTopLeft = '╔';
        private readonly char wallTopRight = '╗';
        private readonly char wallBottomRight = '╝';
        private readonly char wallBottomLeft = '╚';

        public ConsoleGameBoard(IRenderer renderer) 
            : base(renderer)
        {
            this.SetSettings();
        }

        public override void RenderBoard()
        {
            foreach (var wall in this.Walls)
            {
                this.Renderer.Draw(wall, this.Map(wall.Symbol).ToString());
            }
        }

        private char Map(CellType cell) => cell switch
        {
            CellType.WallTopLeft => this.wallTopLeft,
            CellType.WallTopRight => this.wallTopRight,
            CellType.WallBottomLeft => this.wallBottomLeft,
            CellType.WallBottomRight => this.wallBottomRight,
            CellType.WallsTopAndBottom => this.wallsTopAndBottom,
            CellType.WallsLeftAndRight => this.wallsLeftAndRight,
            _ => ' ',
        };

        public override bool SetSettings()
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int totalRows = this.BoardSize.Row;
            int totalCols = this.BoardSize.Col;

            totalCols = Math.Min(totalCols, Console.LargestWindowWidth);
            totalRows = Math.Min(totalRows, Console.LargestWindowHeight);

            Console.SetBufferSize(Math.Max(Console.BufferWidth, totalCols), Math.Max(Console.BufferHeight, totalRows));
            Console.SetWindowSize(totalCols, totalRows);
            Console.SetBufferSize(totalCols, totalRows);
           
            Console.Title = GlobalConstants.SnakeConstants.GameName;
            return true;
        }
    }
}
