namespace SnakeGame.GameObjects
{
    using System.Collections.Generic;

    using SnakeGame.GameObjects.Enums;
    using SnakeGame.GameObjects.Interfaces;
    using SnakeGame.Rendering;

    using static SnakeGame.Common.GlobalConstants.GameConstants;

    public abstract class BaseBoard : IGameBoard
    {
        private readonly List<Coordinates> walls;
        private readonly IRenderer renderer;

        private readonly int topWallRow = HeaderHeight;
        private readonly int bottomWallRow = HeaderHeight + GameHeight + WallsWidth;
        private readonly int leftWallCol = 0;
        private readonly int rightWallCol = GameWidth + WallsWidth;

        protected BaseBoard(IRenderer renderer)
        {
            this.walls = new List<Coordinates>();
            this.BoardSize = new Coordinates(TotalGameWidthRows, TotalGameWidthCols);
            this.renderer = renderer;
            this.Matrix = new CellType[TotalGameWidthRows, TotalGameWidthCols];
        }

        public CellType[,] Matrix { get; private set; }

        public IRenderer Renderer => this.renderer;

        public Coordinates BoardSize { get; }

        public IReadOnlyCollection<Coordinates> Walls => this.walls;

        public IReadOnlyCollection<Coordinates> Coordinates { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CellType[,] GetMatrix => this.Matrix;

        public void CreateBoard()
        {
            this.walls.Clear();

            this.CreateUpDownSide();
            this.CreateLeftRightSide();
        }

        public abstract bool SetSettings();

        private void CreateUpDownSide()
        {
            for (int col = this.leftWallCol; col <= this.rightWallCol; col++)
            {
                var wall1 = new Coordinates(this.topWallRow, col);
                var wall2 = new Coordinates(this.bottomWallRow, col);

                wall1 = this.SetWallSymbol(wall1);
                wall2 = this.SetWallSymbol(wall2);

                this.AddTwoWallsTOMatrix(wall1, wall2);

                this.walls.Add(wall1);
                this.walls.Add(wall2);
            }
        }

        private void CreateLeftRightSide()
        {
            for (int row = this.topWallRow; row < this.bottomWallRow; row++)
            {
                var wall1 = new Coordinates(row, this.leftWallCol);
                var wall2 = new Coordinates(row, this.rightWallCol);

                wall1 = this.SetWallSymbol(wall1);
                wall2 = this.SetWallSymbol(wall2);

                this.AddTwoWallsTOMatrix(wall1, wall2);

                this.walls.Add(wall1);
                this.walls.Add(wall2);
            }
        }

        private void AddTwoWallsTOMatrix(Coordinates coordinates1, Coordinates coordinates2)
        {
            this.Matrix[coordinates1.Row, coordinates1.Col] = coordinates1.Symbol;
            this.Matrix[coordinates2.Row, coordinates2.Col] = coordinates2.Symbol;
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

        public void Add(Coordinates coordinates, CellType cellType = CellType.None)
        {
            // TODO: IsValid
            this.Matrix[coordinates.Row, coordinates.Col] = cellType;
        }

        public void Add(IReadOnlyCollection<Coordinates> coordinates, CellType cellType = CellType.None)
        {
            foreach (var coordinate in coordinates)
            {
                this.Matrix[coordinate.Row, coordinate.Col] = cellType;
            }
        }

        public void RemoveCellType(Coordinates coordinates)
        {
            // TODO: IsValid
            this.Matrix[coordinates.Row, coordinates.Col] = CellType.None;
        }

        public void RemoveAll()
        {
            for (int row = 0; row < this.Matrix.GetLength(0); row++)
            {
                for (int col = 0; col < this.Matrix.GetLength(1); col++)
                {
                    this.Matrix[row, col] = CellType.None;
                }
            }
        }
    }
}
