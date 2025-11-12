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
        private readonly int bottomWallRow =  HeaderHeight + GameHeight + WallsWidth;
        private readonly int leftWallCol = 0;
        private readonly int rightWallCol = GameWidth + WallsWidth;

        protected BaseBoard(IRenderer renderer)
        {
            this.walls = new List<Coordinates>();
            this.BoardSize = new Coordinates(TotalGameWidthRows, TotalGameWidthCols);
            this.renderer = renderer;
        }

        public IRenderer Renderer => this.renderer;

        public Coordinates BoardSize { get; }

        public IReadOnlyCollection<Coordinates> Walls => this.walls;

        public void CreateBorder()
        {
            this.walls.Clear();

            this.CreateUpDownSide();
            this.CreateLeftRightSide();
        }

        public abstract bool SetSettings();

        public abstract void RenderBoard();

        private void CreateUpDownSide()
        {
            for (int col = this.leftWallCol; col <= this.rightWallCol; col++)
            {
                var wall1 = new Coordinates(this.topWallRow, col);
                var wall2 = new Coordinates(this.bottomWallRow, col);

                wall1 = this.SetWallSymbol(wall1);
                wall2 = this.SetWallSymbol(wall2);

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

                this.walls.Add(wall1);
                this.walls.Add(wall2);
            }
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
