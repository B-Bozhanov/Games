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
        private readonly int middleTopWallRow = HeaderHeight;
        private readonly int topWallRow = 0;

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
            this.CreateHeader();
            this.CreateUpDownSide();
            this.CreateLeftRightSide();
        }

        private void CreateHeader()
        {
            // Create top side
            for (int col = WallsWidth; col < this.rightWallCol; col++)
            {
                var wall = new Coordinates(topWallRow, col);

                this.Add(wall, this.GetBorderSymbol(wall));
            }

            // Create left and right sides
            for (int row = this.topWallRow; row < HeaderHeight; row++)
            {
                var leftWall = new Coordinates(row, this.leftWallCol);
                var rightWall = new Coordinates(row, this.rightWallCol);

                this.Add(leftWall, this.GetBorderSymbol(leftWall));
                this.Add(rightWall, this.GetBorderSymbol(rightWall));
            }
        }

        private void CreateLeftRightSide()
        {
            for (int row = this.middleTopWallRow; row < this.bottomWallRow; row++)
            {
                var wall1 = new Coordinates(row, this.leftWallCol);
                var wall2 = new Coordinates(row, this.rightWallCol);

                this.Add(wall1, this.GetBorderSymbol(wall1));
                this.Add(wall2, this.GetBorderSymbol(wall2));
            }
        }

        private void CreateUpDownSide()
        {
            for (int col = this.leftWallCol; col <= this.rightWallCol; col++)
            {
                var wall1 = new Coordinates(this.middleTopWallRow, col);
                var wall2 = new Coordinates(this.bottomWallRow, col);

                this.Add(wall1, this.GetBorderSymbol(wall1));
                this.Add(wall2, this.GetBorderSymbol(wall2));
            }
        }

        private CellType GetBorderSymbol(Coordinates coordinates)
        {
            bool isMiddleTop = coordinates.Row == this.middleTopWallRow;
            bool isBottom = coordinates.Row == this.bottomWallRow;
            bool isLeft = coordinates.Col == this.leftWallCol;
            bool isRight = coordinates.Col == this.rightWallCol;
            bool isTop = coordinates.Row == this.topWallRow;

            if (isTop && isRight) return CellType.WallTopRight;
            if (isTop && isLeft) return CellType.WallTopLeft;
            if (isMiddleTop && isLeft) return CellType.WallMiddleLeft;
            if (isMiddleTop && isRight) return CellType.WallMiddleRight;
            if (isBottom && isLeft) return CellType.WallBottomLeft;
            if (isBottom && isRight) return CellType.WallBottomRight;
            if (isMiddleTop || isBottom || isTop) return CellType.WallsTopAndBottom;

            return CellType.WallsLeftAndRight;
        }
    }
}
