namespace SnakeGame.Core.GameLoop
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using SnakeGame.Core.GameLoop.Interfaces;
    using SnakeGame.Core.State;
    using SnakeGame.Extensions;
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.Input.Enums;

    public sealed class GameEngine : IGameEngine
    {
        private readonly IGameBoard gameBoard;
        private readonly IObjectFactory objectFactory;
        private IDictionary<Coordinates, Obstacle> obstacles;
        private GameState? gameState;

        public GameEngine(IGameBoard gameBoard, IObjectFactory objectFactory)
        {
            this.gameBoard = gameBoard;
            this.objectFactory = objectFactory;

            this.obstacles = new Dictionary<Coordinates, Obstacle>();
        }

        public void FixedUpdate(GameState gameState, IReadOnlyDictionary<SnakeId, Direction> decisions, double deltaSeconds)
        {
            this.gameState = gameState;
            var players = gameState.Players;
            var food = gameState.Food!;

            foreach (var (id, player) in players)
            {
                player.MoveTimer += deltaSeconds;
                if (player.MoveTimer < player.MoveIntervalSeconds)
                {
                    continue;
                }

                player.MoveTimer = 0;

                var direction = gameState.PendingKey;
                var nextHead = player.Snake.GetNextHeadPossition(direction);

                if (this.WillDie(nextHead) || player.Snake.WillCollideWithSelf(nextHead))
                {
                    player.IsAlive = false;
                    gameState.IsGameOver = true;
                }

                this.Eat(player, ref food, nextHead);
                if (food.IsExpired)
                {
                    food = this.UpdateFood(food);
                }

                this.UpdateObstacles();
                this.UpdateSnake(direction, player.Snake, nextHead);


                this.gameBoard.Add(player.Snake.Body, CellType.SnakeBody);
                this.gameBoard.Add(player.Snake.HeadPossition, player.Snake.NextHeadPossitionSymbol);
                this.gameBoard.Add(player.Snake.GetCurrentTailPossition, player.Snake.NextTailPossitionSymbol);

                if (!player.Snake.ShouldEat)
                {
                    this.gameBoard.RemoveCellType(player.Snake.GetLastTailPossition);
                }

                gameState.Food = food;
            }
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

        private void UpdateObstacles()
        {
            var expiredKeys = new List<Coordinates>();

            foreach (var o in this.gameState.Obstacles)
            {
                if (o.Value.IsExpired)
                {
                    this.gameBoard.RemoveCellType(o.Key);
                    this.gameState!.UnBlock(o.Key);
                    expiredKeys.Add(o.Key);
                }
            }

            if (expiredKeys.Count == 0) return;

            this.gameState.Obstacles.RemoveRange(expiredKeys);

            var newObstacles = this.objectFactory.CreateObstacles(
                expiredKeys.Count,
                this.gameBoard.BoardConfig,
                this.gameState!.BlockList);

            foreach (var kvp in newObstacles)
            {
                this.gameState.Obstacles.Add(kvp);
                this.gameState!.Block(kvp.Key);
                this.gameBoard.Add(kvp.Key, CellType.Obstacle);
            }
        }

        private void UpdateSnake(Direction direction, ISnake snake, Coordinates nextHead)
        {
            this.gameState!.UnBlock(snake.Body);
            snake.Move(direction);
            this.gameState!.Block(snake.Body);
            this.gameState!.Block(nextHead);
        }

        private bool WillDie(Coordinates nextHead)
                 => this.WillHitObstacle(nextHead)
                 || !nextHead.IsInRange(this.gameBoard.BoardConfig.TotalRows, this.gameBoard.BoardConfig.TotalCols);

        private bool WillHitObstacle(Coordinates nextHead)
            => this.gameState.Obstacles.ContainsKey(key: nextHead);
    }
}
