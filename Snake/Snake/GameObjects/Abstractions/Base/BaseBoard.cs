namespace SnakeGame.GameObjects.Abstractions.Base
{
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    using static SnakeGame.Common.GlobalConstants.GameConstants;

    public abstract class BaseBoard : IGameBoard
    {
        private readonly int bottomWallRow = HeaderHeight + PlayableBoardHeight + WallsWidth;
        private readonly int leftWallCol = 0;
        private readonly int rightWallCol = PlayableBoardWidth + WallsWidth;
        private readonly int topWallRow = HeaderHeight;

        protected BaseBoard(IBoardConfig boardConfig)
        {
            this.BoardConfig = boardConfig;
            this.Board = new CellType[this.BoardConfig.TotalRows, this.BoardConfig.TotalCols];
        }

        public CellType[,] Board { get; private set; }

        public CellType[,] GetBoard => this.Board;

        public IBoardConfig BoardConfig { get; }

        public void Add(Coordinates coordinates, CellType cellType = CellType.None)
        {
            // TODO: IsValid
            this.Board[coordinates.Row, coordinates.Col] = cellType;
        }

        public void Add(IReadOnlyCollection<Coordinates> coordinates, CellType cellType = CellType.None)
        {
            foreach (var coordinate in coordinates)
            {
                this.Board[coordinate.Row, coordinate.Col] = cellType;
            }
        }

        public void RemoveCellType(Coordinates coordinates)
        {
            // TODO: IsValid
            this.Board[coordinates.Row, coordinates.Col] = CellType.None;
        }

        public void RemoveAll()
        {
            Array.Clear(this.Board, 0, this.Board.Length);
        }

        public abstract bool SetSettings();

        public void CreateBoard()
        {
            this.CreateUpDownSide();
            this.CreateLeftRightSide();
        }

        private void CreateLeftRightSide()
        {
            for (int row = this.topWallRow; row < this.bottomWallRow; row++)
            {
                var wall1 = new Coordinates(row, this.leftWallCol);
                var wall2 = new Coordinates(row, this.rightWallCol);

                wall1 = this.SetWallSymbol(wall1);
                wall2 = this.SetWallSymbol(wall2);

                this.AddTwoWalls(wall1, wall2);
            }
        }

        private void CreateUpDownSide()
        {
            for (int col = this.leftWallCol; col <= this.rightWallCol; col++)
            {
                var wall1 = new Coordinates(this.topWallRow, col);
                var wall2 = new Coordinates(this.bottomWallRow, col);

                wall1 = this.SetWallSymbol(wall1);
                wall2 = this.SetWallSymbol(wall2);

                this.AddTwoWalls(wall1, wall2);
            }
        }

        private void AddTwoWalls(Coordinates coordinates1, Coordinates coordinates2)
        {
            this.Board[coordinates1.Row, coordinates1.Col] = coordinates1.Symbol;
            this.Board[coordinates2.Row, coordinates2.Col] = coordinates2.Symbol;
        }

        private CellType GetBorderSymbol(Coordinates coordinates)
        {
            bool isTop = coordinates.Row == this.topWallRow;
            bool isBottom = coordinates.Row == this.bottomWallRow;
            bool isLeft = coordinates.Col == this.leftWallCol;
            bool isRight = coordinates.Col == this.rightWallCol;

            if (isTop && isLeft) return CellType.WallTopLeft;
            if (isTop && isRight) return CellType.WallTopRight;
            if (isBottom && isLeft) return CellType.WallBottomLeft;
            if (isBottom && isRight) return CellType.WallBottomRight;
            if (isTop || isBottom) return CellType.WallsTopAndBottom;

            return CellType.WallsLeftAndRight;
        }

        private Coordinates SetWallSymbol(Coordinates wall)
        {
            wall.Symbol = this.GetBorderSymbol(wall);
            return wall;
        }
    }
}
