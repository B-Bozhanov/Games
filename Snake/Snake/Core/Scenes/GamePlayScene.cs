namespace SnakeGame.Core.Scenes
{
    using System.Collections.Generic;
    using System.Text.Json;

    using Microsoft.Extensions.DependencyInjection;

    using SnakeGame.Core.GameLoop.Interfaces;
    using SnakeGame.Core.Scenes.Interfaces;
    using SnakeGame.Extensions;
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Base;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.Input;
    using SnakeGame.Input.Enums;
    using SnakeGame.Rendering;
    using SnakeGame.Services;
    using SnakeGame.SnakeAI;

    /// <summary>
    /// Main orchestrator of the Snake gameplay loop.
    /// Coordinates: input → movement → spawning → board updates → rendering.
    /// Uses double-buffer rendering and a block-list to maintain valid spawn positions.
    /// </summary>
    public class GameplayScene : IGameScene
    {
        private readonly ISnake snake;
        private readonly ISnake snakeEnemy;
        private readonly IRenderer renderer;
        private readonly IObjectFactory objectFactory;
        private readonly int obstaclesCount = 2;
        private readonly IInputReader inputReader;
        private readonly IGameTime gameTime;
        private readonly IGameBoard gameBoard;
        private readonly ISnakeAiController aiController;

        // blockList keeps track of occupied cells (snake, food, obstacles).
        // Used by factories to guarantee valid spawn positions without scanning the board.
        private readonly bool[,] blockList;
        private CellType[,] prevScene;
        private CellType[,] currScene;
        private int currentSpeed = 1;
        private IDictionary<Coordinates, Obstacle> obstacles;
        private readonly bool isAutoPlay = true;
        private readonly bool isMultiPlayer = false;

        public GameplayScene(
                IInputReader inputReader,
                IGameTime gameTime,
                IRenderer renderer,
                IObjectFactory objectFactory,
                IGameBoard gameBoard,
                ISnakeAiController aiController,
                [FromKeyedServices("snake")] ISnake snake,
                [FromKeyedServices("snakeEnimy")] ISnake snakeEnimy)
        {
            this.inputReader = inputReader;
            this.gameTime = gameTime;
            this.renderer = renderer;
            this.objectFactory = objectFactory;
            this.snake = snake;
            this.snakeEnemy = snakeEnimy;
            this.gameBoard = gameBoard;
            this.aiController = aiController;
            var rows = this.gameBoard.BoardConfig.TotalRows;
            var cols = this.gameBoard.BoardConfig.TotalCols;
            this.blockList = new bool[rows, cols];
            this.prevScene = new CellType[rows, cols];
            this.currScene = new CellType[rows, cols];
            this.obstacles = new Dictionary<Coordinates, Obstacle>();
        }
        private double snakeMoveIntervalSeconds = 0.15; // колко често да се мести
        private double snakeTimer = 0;
        public void Run()
        {
            var food = this.InitialGame();

            this.prevScene = (CellType[,])this.gameBoard.GetBoard.Clone();
            this.renderer.Draw(prevScene);

            while (true)
            {
                // Maintain frame pacing (FPS control).
                this.gameTime.Tick();

                Direction direction = Direction.None;
                Direction aiDirection = Direction.None;
                Coordinates nextHead = new();
                Coordinates enemyNextHead = new();

                if (this.isAutoPlay)
                {
                    aiDirection = this.aiController.GetNextDirection(
                        this.gameBoard,
                        this.snakeEnemy.HeadPossition,
                        food.Coordinates,
                        this.snakeEnemy.Body);
                    enemyNextHead = this.snakeEnemy.GetNextHeadPossition(aiDirection);
                    if (this.WillDie(enemyNextHead) || this.snakeEnemy.WillCollideWithSelf(enemyNextHead))
                    {
                        var reason = string.Empty;
                        if (this.WillHitObstacle(enemyNextHead))
                        {
                            reason = "Hit Obstacle";
                        }
                        else if (this.snakeEnemy.WillCollideWithSelf(enemyNextHead))
                        {
                            reason = "Collide it self";
                        }
                        else
                        {
                            reason = "Hit Wall";
                        }

                        AiLogger.LogDeath(
                              this.gameTime.CurrentFps,
                              this.snakeEnemy.HeadPossition,
                              enemyNextHead,
                              reason);
                        Console.Write("Enemy - Game Over");
                        break;
                    }
                    snakeTimer += gameTime.DeltaTimeSeconds;


                    if (snakeTimer >= snakeMoveIntervalSeconds)
                    {
                        this.UpdateSnake(aiDirection, this.snakeEnemy, enemyNextHead);
                        snakeTimer = 0;
                    }

                    this.Eat(this.snakeEnemy, ref food, enemyNextHead);
                    if (food.IsExpired)
                    {
                        food = this.UpdateFood(food);
                    }
                    // Keep obstacle count constant: remove expired ones and spawn replacements
                    // using blockList to ensure valid positions.
                    this.UpdateObstacles();
                    this.gameBoard.Add(this.snakeEnemy.Body, CellType.SnakeBody);
                    this.gameBoard.Add(this.snakeEnemy.HeadPossition, this.snakeEnemy.NextHeadPossitionSymbol);
                    this.gameBoard.Add(this.snakeEnemy.GetCurrentTailPossition, this.snakeEnemy.NextTailPossitionSymbol);

                    if (!this.snakeEnemy.ShouldEat)
                    {
                        this.gameBoard.RemoveCellType(this.snakeEnemy.GetLastTailPossition);
                    }
                    Console.SetCursorPosition(3, 2);
                    Console.Write($"Fps = {gameTime.CurrentFps}");
                    var speed = snakeMoveIntervalSeconds;
                    Console.SetCursorPosition(100, 2);
                    Console.Write($"Speed = {speed:F2}");
                    Console.SetCursorPosition(45, 2);
                    Console.Write($"Speed = {snakeTimer}");

                }
                else if (isMultiPlayer)
                {
                    nextHead = this.snake.GetNextHeadPossition(direction);
                    enemyNextHead = this.snakeEnemy.GetNextHeadPossition(aiDirection);
                    if (this.WillDie(nextHead) || snake.WillCollideWithSelf(nextHead))
                    {
                        Console.Write("Game Over");
                        break;
                    }
                    if (this.WillDie(enemyNextHead) || this.snakeEnemy.WillCollideWithSelf(enemyNextHead))
                    {
                        Console.Write("Enemy - Game Over");
                        break;
                    }


                    this.UpdateSnake(direction, this.snake, nextHead);

                    this.UpdateSnake(aiDirection, this.snakeEnemy, enemyNextHead);
                    this.Eat(this.snake, ref food, nextHead);
                    this.Eat(this.snakeEnemy, ref food, enemyNextHead);

                    this.gameBoard.Add(this.snake.Body, CellType.SnakeBody);
                    this.gameBoard.Add(nextHead, this.snake.NextHeadPossitionSymbol);
                    this.gameBoard.Add(snake.GetCurrentTailPossition, this.snake.NextTailPossitionSymbol);

                    this.gameBoard.Add(this.snakeEnemy.Body, CellType.SnakeBody);
                    this.gameBoard.Add(enemyNextHead, this.snakeEnemy.NextHeadPossitionSymbol);
                    this.gameBoard.Add(this.snakeEnemy.GetCurrentTailPossition, this.snakeEnemy.NextTailPossitionSymbol);

                    if (!this.snakeEnemy.ShouldEat)
                    {
                        this.gameBoard.RemoveCellType(this.snakeEnemy.GetLastTailPossition);
                    }
                    if (!this.snake.ShouldEat)
                    {
                        this.gameBoard.RemoveCellType(this.snake.GetLastTailPossition);
                    }
                }
                else
                {
                    KeyPressed input = this.inputReader.GetInput();
                    direction = DirectionService.GetByPressedKey(input);
                    nextHead = this.snake.GetNextHeadPossition(direction);
                    if (this.WillDie(nextHead) || snake.WillCollideWithSelf(nextHead))
                    {
                        Console.Write("Game Over");
                        break;
                    }


                    this.UpdateSnake(direction, this.snake, nextHead);
                    this.Eat(this.snake, ref food, nextHead);
                    this.gameBoard.Add(this.snake.Body, CellType.SnakeBody);
                    this.gameBoard.Add(nextHead, this.snake.NextHeadPossitionSymbol);
                    this.gameBoard.Add(snake.GetCurrentTailPossition, this.snake.NextTailPossitionSymbol);

                    if (!this.snake.ShouldEat)
                    {
                        this.gameBoard.RemoveCellType(this.snake.GetLastTailPossition);
                    }
                }

                this.currScene = (CellType[,])this.gameBoard.GetBoard.Clone();

                // Render only the diff between previous and current frame (double-buffer drawing).
                this.renderer.Draw(prevScene, currScene);

                // Swap buffers (prev ↔ curr) to enable flicker-free differential rendering.
                (this.currScene, this.prevScene) = (this.prevScene, this.currScene);

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

        private Food HandleFoodEaten(Food oldFood, ISnake snake)
        {
            snake.Eat();
            return this.UpdateFood(oldFood);
        }

        private Food InitialGame()
        {
            this.gameBoard.CreateBoard();

            this.gameBoard.Add(snake.Body);
            this.Block(this.snake.Body);

            if (this.isAutoPlay || this.isMultiPlayer)
            {
                this.gameBoard.Add(snakeEnemy.Body);
                this.Block(this.snakeEnemy.Body);
            }

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
            this.gameBoard.Add(obsCoordinates!, CellType.Obstacle);

            return food;
        }

        private bool IsBlocked(Coordinates coordinates)
            => this.blockList[coordinates.Row, coordinates.Col];

        private void Eat(ISnake snake, ref Food food, Coordinates nextHead)
        {
            if (nextHead == food!.Coordinates)
            {
                food = this.HandleFoodEaten(food, snake);
                if (this.snakeMoveIntervalSeconds > 0.01)
                {
                    this.snakeMoveIntervalSeconds -= 0.01;
                }
            }
        }

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

        private void UpdateSnake(Direction direction, ISnake snake, Coordinates nextHead)
        {
            this.UnBlock(snake.Body);
            snake.Move(direction);
            this.Block(snake.Body);
            this.Block(nextHead);
        }

        private bool WillDie(Coordinates nextHead)
                 => this.WillHitObstacle(nextHead)
                 || !nextHead.IsInRange(this.gameBoard.BoardConfig.TotalRows, this.gameBoard.BoardConfig.TotalCols);

        private bool WillHitObstacle(Coordinates nextHead)
            => this.obstacles.ContainsKey(key: nextHead);
    }
}