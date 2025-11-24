namespace SnakeGame.Core.GameLoop
{
    using System.Collections.Generic;

    using SnakeGame.Core.GameLoop.Interfaces;
    using SnakeGame.Core.State;
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public sealed class GameEngine : IGameEngine
    {
        private readonly IGameBoard gameBoard;
        private readonly IObjectFactory objectFactory;
        private GameState? gameState;

        public GameEngine(IGameBoard gameBoard, IObjectFactory objectFactory)
        {
            this.gameBoard = gameBoard;
            this.objectFactory = objectFactory;
        }

        public void FixedUpdate(GameState gameState, IReadOnlyDictionary<SnakeId, Direction> decisions, double deltaSeconds)
        {
            this.gameState = gameState;
            throw new NotImplementedException();
        }

        private void Eat(Player player, ref Food food, Coordinates nextHead)
        {
            if (nextHead == food!.Coordinates)
            {
                food = this.HandleFoodEaten(food, player.Snake);
                if (player.MoveIntervalSeconds > 0.01)
                {
                    player.MoveIntervalSeconds -= 0.01;
                }

                player.Score++;
            }
        }

        private Food HandleFoodEaten(Food oldFood, ISnake snake)
        {
            snake.Eat();
            return this.UpdateFood(oldFood);
        }

        private Food UpdateFood(Food oldFood)
        {
            this.gameBoard.RemoveCellType(oldFood.Coordinates);
            this.gameState!.UnBlock(oldFood.Coordinates);

            var newFood = this.objectFactory.CreateFood(this.gameState.BoardConfig, this.gameState.BlockList);
            this.gameBoard.Add(newFood.Coordinates, CellType.Food);
            this.gameState.Block(newFood.Coordinates);

            return newFood;
        }
    }
}
