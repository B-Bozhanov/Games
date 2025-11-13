namespace SnakeGame.GameObjects
{
    using System;

    using SnakeGame.Common;

    public class ConsoleGameBoard : BaseBoard
    {
        public ConsoleGameBoard()
        {
            this.SetSettings();
        }

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
