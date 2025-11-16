namespace SnakeGame.GameObjects
{
    using SnakeGame.GameObjects.Abstractions.Base;

    using static SnakeGame.Common.GlobalConstants;

    public class SnakeEnimy : BaseSnake
    {
        public SnakeEnimy(int startPossition = SnakeConstants.StartPossitionRow, int length = SnakeConstants.DefaultLength) 
            : base(startPossition, length)
        {
            this.CurrentDirection = SnakeConstants.DefaultSnakeEnemyDirection;
            for (int i = 0; i < this.body.Count - 2; i++)
            {
                this.tailDirection.Enqueue(this.CurrentDirection);
            }
        }

        protected override void InitialBody(int startPossitionRow, int length)
        {
            for (int col = 0; col < length; col++)
            {
                this.body.Enqueue(new Coordinates(startPossitionRow, 120 - col));
            }
        }
    }
}
