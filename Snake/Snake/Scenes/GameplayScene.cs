namespace SnakeGame.Scenes
{
    using System.Collections.Generic;
    using System.Text.Json;

    using Microsoft.Extensions.DependencyInjection;

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
        private readonly ISnake snakeEnimy;
        private readonly IRenderer renderer;
        private readonly IObjectFactory objectFactory;
        private readonly int obstaclesCount = 100;
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
        private readonly bool useAi = true;

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
            this.snakeEnimy = snakeEnimy;
            this.gameBoard = gameBoard;
            this.aiController = aiController;
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

            int count = 0;
            int point1 = 0;
            int point2 = 0;

            while (true)
            {
                Direction humanDir;
                Direction aiDir = Direction.Right;

                if (this.useAi && this.aiController is not null)
                {
                    //var total = new Queue<Coordinates>();

                    //foreach (var item in snakeEnimy.Body)
                    //{
                    //    total.Enqueue(item);
                    //}
                    //foreach (var item in snake.Body)
                    //{
                    //    total.Enqueue(item);
                    //}
                    //var context = new SnakeAiContext(
                    //    this.snakeEnimy.HeadPossition,
                    //    food.Coordinates,
                    //    snakeEnimy.Body,
                    //    this.gameBoard);

                   // aiDir = this.aiController.GetNextDirection(context);

                    //this.LogJson(context, aiDir);
                }

                //humanDir = this.inputReader.GetInput();


                //var nexHead = this.snake.GetNextHeadPossition(humanDir);
                var enemyNextHead = this.snakeEnimy.GetNextHeadPossition(aiDir);

                //if (this.WillDie(nexHead) || snake.WillCollideWithSelf(nexHead))
                //{
                //    Console.Write("Game Over");
                //    break;
                //}

                if (this.WillDie(enemyNextHead) || this.snakeEnimy.WillCollideWithSelf(enemyNextHead))
                {
                    Console.Write("Enemy - Game Over");
                    break;
                }

                //this.UpdateSnake(humanDir, this.snake);
                this.UpdateSnake(aiDir, this.snakeEnimy, enemyNextHead);

                //if (nexHead == food!.Coordinates)
                //{
                //    food = this.HandleFoodEaten(food, this.snake);
                //    if ((count + 10) % 2 == 0)
                //    {
                //        this.gameTime.IncreaseSpeed();
                //    }
                //    Console.SetCursorPosition(2, 2);
                //    point1++;
                //    Console.Write(point1);
                //}

                if (enemyNextHead == food!.Coordinates)
                {
                    food = this.HandleFoodEaten(food, this.snakeEnimy);
                   
                        this.gameTime.IncreaseSpeed();
                    
                    Console.SetCursorPosition(119, 2);
                    point2++;
                    Console.Write(point2);
                }

                if (food.IsExpired)
                {
                    food = this.UpdateFood(food);
                }

                // Keep obstacle count constant: remove expired ones and spawn replacements
                // using blockList to ensure valid positions.
                this.UpdateObstacles();

                //this.gameBoard.Add(this.snake.Body, CellType.SnakeBody);
                //this.gameBoard.Add(nexHead, this.snake.NextHeadPossitionSymbol);
                //this.gameBoard.Add(snake.GetCurrentTailPossition, this.snake.NextTailPossitionSymbol);

                this.gameBoard.Add(this.snakeEnimy.Body, CellType.SnakeBody);
                this.gameBoard.Add(enemyNextHead, this.snakeEnimy.NextHeadPossitionSymbol);
                this.gameBoard.Add(this.snakeEnimy.GetCurrentTailPossition, this.snakeEnimy.NextTailPossitionSymbol);

                if (!this.snake.ShouldEat)
                {
                    this.gameBoard.RemoveCellType(this.snake.GetLastTailPossition);
                }
                if (!this.snakeEnimy.ShouldEat)
                {
                    this.gameBoard.RemoveCellType(this.snakeEnimy.GetLastTailPossition);
                }

                this.currScene = (CellType[,])this.gameBoard.GetBoard.Clone();

                // Render only the diff between previous and current frame (double-buffer drawing).
                this.renderer.Draw(prevScene, currScene);

                // Swap buffers (prev ↔ curr) to enable flicker-free differential rendering.
                (this.currScene, this.prevScene) = (this.prevScene, this.currScene);

                count++;
                Console.SetCursorPosition(55, 2);
                Console.Write(this.gameTime.CurrentFps);
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

        private Food HandleFoodEaten(Food oldFood, ISnake snake)
        {
            snake.Eat();
            return this.UpdateFood(oldFood);
        }

        private Food InitialGame()
        {
            this.gameBoard.CreateBoard();

            this.gameBoard.Add(snakeEnimy.Body);
            //this.Block(this.snake.Body);
            this.Block(this.snakeEnimy.Body);

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
            //this.gameBoard.Add(snake.Body, CellType.SnakeBody);
            this.gameBoard.Add(obsCoordinates!, CellType.Obstacle);

            return food;
        }

        private bool IsBlocked(Coordinates coordinates)
            => this.blockList[coordinates.Row, coordinates.Col];

        private void LogJson(SnakeAiContext context, Direction dir)
        {
            var data = new
            {
                Head = new { context.Head.Row, context.Head.Col },
                Food = new { context.Food.Row, context.Food.Col },
                Body = context.Body.Select(b => new { b.Row, b.Col }),
                Obstacles = this.obstacles.Keys,
                Tick = this.gameTime.CurrentFps,
                Direction = dir.ToString()
            };

            var json = JsonSerializer.Serialize(data);

            File.AppendAllText("snake_ai_log.json", json + Environment.NewLine);
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
                new Random().Next(1, 10),
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