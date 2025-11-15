namespace SnakeGame.Scenes
{
    using System.Collections.Generic;

    using SnakeGame.Core;
    using SnakeGame.Extensions;
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.Input;
    using SnakeGame.Rendering;

    /// <summary>
    /// Main orchestrator of the Snake gameplay loop.
    /// Coordinates: input → movement → spawning → board updates → rendering.
    /// Uses double-buffer rendering and a block-list to maintain valid spawn positions.
    /// </summary>
    public class GameplayScene : IGameScene
    {
        private readonly ISnake snake;
        private readonly IRenderer renderer;
        private readonly IObjectFactory objectFactory;
        private readonly int obstaclesCount = 3;
        private readonly IInputReader inputReader;
        private readonly IGameTime gameTime;
        private readonly IGameBoard gameBoard;
        // blockList keeps track of occupied cells (snake, food, obstacles).
        // Used by factories to guarantee valid spawn positions without scanning the board.
        private readonly bool[,] blockList;
        private CellType[,] prevScene;
        private CellType[,] currScene;
        private int currentSpeed = 1;
        private IDictionary<Coordinates, Obstacle> obstacles;

        public GameplayScene(
                IInputReader inputReader,
                IGameTime gameTime,
                IRenderer renderer,
                IObjectFactory objectFactory,
                ISnake snake,
                IGameBoard gameBoard)
        {
            this.inputReader = inputReader;
            this.gameTime = gameTime;
            this.renderer = renderer;
            this.objectFactory = objectFactory;
            this.snake = snake;
            this.gameBoard = gameBoard;

            var rows = this.gameBoard.BoardConfig.TotalRows;
            var cols = this.gameBoard.BoardConfig.TotalCols;
            this.blockList = new bool[rows, cols];
            this.prevScene = new CellType[rows, cols];
            this.currScene = new CellType[rows, cols];
            this.obstacles = new Dictionary<Coordinates, Obstacle>();
        }

        public void Run()
        {
            var food = this.InitialGame();

            this.prevScene = (CellType[,])this.gameBoard.GetBoard.Clone();

            this.renderer.Draw(prevScene);

            while (true)
            {
                var direction = this.inputReader.GetInput();
                var nexHead = snake.GetNextHeadPossition(direction);

                if (this.WillDie(nexHead) || snake.WillCollideWithSelf(nexHead))
                {
                    Console.Write("Game Over");
                    break;
                }

                this.UpdateSnake(direction);

                if (nexHead == food!.Coordinates)
                {
                    food = this.HandleFoodEaten(food);
                    this.gameTime.IncreaseSpeed();
                    currentSpeed++;
                }

                if (food.IsExpired)
                {
                    food = this.UpdateFood(food);
                }

                // Keep obstacle count constant: remove expired ones and spawn replacements
                // using blockList to ensure valid positions.
                this.UpdateObstacles();

                this.gameBoard.Add(this.snake.Body, CellType.SnakeBody);

                if (!this.snake.ShouldEat)
                {
                    this.gameBoard.RemoveCellType(this.snake.GetLastTailPossition);
                }

                this.currScene = (CellType[,])this.gameBoard.GetBoard.Clone();

                // Render only the diff between previous and current frame (double-buffer drawing).
                this.renderer.Draw(prevScene, currScene);

                // Swap buffers (prev ↔ curr) to enable flicker-free differential rendering.
                (this.currScene, this.prevScene) = (this.prevScene, this.currScene);

                // Maintain frame pacing (FPS control).
                this.gameTime.Tick();
            }
        }

        private void Block(Coordinates coordinates)
            => this.blockList[coordinates.Row, coordinates.Col] = true;

        private void Block(IReadOnlyCollection<Coordinates> coordinates)
        {
            foreach (var c in coordinates)
            {
                this.blockList[c.Row, c.Col] = true;
            }
        }

        private Food HandleFoodEaten(Food oldFood)
        {
            this.snake.Eat();
            return this.UpdateFood(oldFood);
        }

        private Food InitialGame()
        {
            this.gameBoard.CreateBoard();

            this.Block(snake.Body);

            var food = this.objectFactory.CreateFood(
                this.gameBoard.BoardConfig,
                this.blockList);
            this.Block(food.Coordinates);

            this.obstacles = this.objectFactory.CreateObstacles(
                this.obstaclesCount,
                this.gameBoard.BoardConfig,
                this.blockList);

            var obsCoordinates = this.obstacles.Keys as IReadOnlyCollection<Coordinates>;
            this.Block(obsCoordinates!);

            this.gameBoard.Add(food.Coordinates, CellType.Food);
            this.gameBoard.Add(snake.Body, CellType.SnakeBody);
            this.gameBoard.Add(obsCoordinates!, CellType.Obstacle);

            return food;
        }

        private bool IsBlocked(Coordinates coordinates)
            => this.blockList[coordinates.Row, coordinates.Col];

        private void UnBlock(Coordinates coordinates)
                    => this.blockList[coordinates.Row, coordinates.Col] = false;

        private void UnBlock(IReadOnlyCollection<Coordinates> coordinates)
        {
            foreach (var c in coordinates)
            {
                this.blockList[c.Row, c.Col] = false;
            }
        }

        private Food UpdateFood(Food oldFood)
        {
            this.gameBoard.RemoveCellType(oldFood.Coordinates);
            this.UnBlock(oldFood.Coordinates);

            var newFood = this.objectFactory.CreateFood(this.gameBoard.BoardConfig, blockList);
            this.gameBoard.Add(newFood.Coordinates, CellType.Food);
            this.Block(newFood.Coordinates);

            return newFood;
        }

        private void UpdateObstacles()
        {
            var expiredKeys = new List<Coordinates>();

            foreach (var o in this.obstacles)
            {
                if (o.Value.IsExpired)
                {
                    this.gameBoard.RemoveCellType(o.Key);
                    this.UnBlock(o.Key);
                    expiredKeys.Add(o.Key);
                }
            }

            if (expiredKeys.Count == 0) return;

            this.obstacles.RemoveRange(expiredKeys);

            var newObstacles = this.objectFactory.CreateObstacles(
                expiredKeys.Count,
                this.gameBoard.BoardConfig,
                this.blockList);

            foreach (var kvp in newObstacles)
            {
                this.obstacles.Add(kvp);
                this.Block(kvp.Key);
                this.gameBoard.Add(kvp.Key, CellType.Obstacle);
            }
        }

        private void UpdateSnake(Direction direction)
        {
            this.UnBlock(this.snake.Body);
            this.snake.Move(direction);
            this.Block(this.snake.Body);
        }

        private bool WillDie(Coordinates nextHead)
                 => this.WillHitObstacle(nextHead)
                 || !nextHead.IsInRange(this.gameBoard.BoardConfig.TotalRows, this.gameBoard.BoardConfig.TotalCols);

        private bool WillHitObstacle(Coordinates nextHead)
            => this.obstacles.ContainsKey(key: nextHead);
    }
}