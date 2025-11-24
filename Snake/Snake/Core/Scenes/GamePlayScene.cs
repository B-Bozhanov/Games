namespace SnakeGame.Core.Scenes
{
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    using SnakeGame.Core.GameLoop.ENums;
    using SnakeGame.Core.GameLoop.Interfaces;
    using SnakeGame.Core.Scenes.Interfaces;
    using SnakeGame.Core.State;
    using SnakeGame.Extensions;
    using SnakeGame.GameObjects;
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
        private GameState gameState;

        private readonly GameMode gameMode;

        // Колекция от играчи (SnakeId -> SnakePlayer).
        // Засега ще я ползваме само за да държим двамата играчи,
        // но е готова за истински мултиплеър с N играча.
        private readonly Dictionary<SnakeId, Player> players = [];

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

        public GameplayScene(
                IInputReader inputReader,
                IGameTime gameTime,
                IRenderer renderer,
                IObjectFactory objectFactory,
                IGameBoard gameBoard,
                ISnakeAiController aiController,
                GameMode gameMode = GameMode.SingleAi)
        {
            this.gameMode = gameMode;
            this.inputReader = inputReader;
            this.gameTime = gameTime;
            this.renderer = renderer;
            this.objectFactory = objectFactory;
            this.gameBoard = gameBoard;
            this.aiController = aiController;
            var rows = this.gameBoard.BoardConfig.TotalRows;
            var cols = this.gameBoard.BoardConfig.TotalCols;
            this.blockList = new bool[rows, cols];
            this.prevScene = new CellType[rows, cols];
            this.currScene = new CellType[rows, cols];
            this.obstacles = new Dictionary<Coordinates, Obstacle>();

            this.gameState = new(this.gameBoard.BoardConfig);
        }

        public void Run()
        {
            var food = this.InitialGame();

            this.gameState!.Food = food;

            this.prevScene = (CellType[,])this.gameBoard.GetBoard.Clone();
            this.renderer.Draw(prevScene);

            while (true)
            {
                // Maintain frame pacing (FPS control).
                this.gameTime.Tick();

                var keyPressed = KeyPressed.None;

                foreach (var kvp in this.gameState.Players)
                {

                    var player = kvp.Value;
                    var snake = player.Snake;
                    if (!player.IsAlive)
                    {
                        continue;
                    }

                    player.MoveTimer += this.gameTime.DeltaTimeSeconds;
                    if (player.MoveTimer < player.MoveIntervalSeconds)
                    {
                        continue;
                    }

                    player.MoveTimer = 0;

                    if (player.Type == PlayerType.Human)
                    {
                        keyPressed = this.inputReader.GetInput();
                    }
                    var direction = this.ResolveDirection(player, snake.CurrentDirection, food, keyPressed);
                    var nextHead = snake.GetNextHeadPossition(direction);

                    if (this.WillDie(nextHead) || snake.WillCollideWithSelf(nextHead))
                    {
                        player.IsAlive = false;
                        continue;
                    }


                    // Keep obstacle count constant: remove expired ones and spawn replacements
                    // using blockList to ensure valid positions.
                    this.Eat(player, ref food, nextHead);
                    if (food.IsExpired)
                    {
                        food = this.UpdateFood(food);
                    }

                    this.UpdateObstacles();
                    this.UpdateSnake(direction, snake, nextHead);


                    this.gameBoard.Add(snake.Body, CellType.SnakeBody);
                    this.gameBoard.Add(snake.HeadPossition, snake.NextHeadPossitionSymbol);
                    this.gameBoard.Add(snake.GetCurrentTailPossition, snake.NextTailPossitionSymbol);

                    if (!snake.ShouldEat)
                    {
                        this.gameBoard.RemoveCellType(snake.GetLastTailPossition);
                    }

                    var speed = player.MoveIntervalSeconds;
                    Console.SetCursorPosition(100, 2);
                    Console.Write($"Speed = {speed:F2}");
                    Console.SetCursorPosition(45, 2);
                    Console.Write($"Score = {player.Score}");
                }

                Console.SetCursorPosition(3, 2);
                Console.Write($"Fps = {gameTime.CurrentFps}");


                this.currScene = (CellType[,])this.gameBoard.GetBoard.Clone();

                // Render only the diff between previous and current frame (double-buffer drawing).
                this.renderer.Draw(prevScene, currScene);

                // Swap buffers (prev ↔ curr) to enable flicker-free differential rendering.
                (this.currScene, this.prevScene) = (this.prevScene, this.currScene);
            }
        }

        private SnakeId GetSnakeId()
        {
            var random = new Random();
            var id = random.Next(1, 1000);
            return new SnakeId(id);
        }

        private void InitializePlayers()
        {
            switch (this.gameMode)
            {
                case GameMode.SinglePlayer:
                    var id1 = this.GetSnakeId();
                    this.gameState.Players[id1] = new Player(
                        name: "Player1",
                        snakeId: id1,
                        type: PlayerType.Human,
                        snake: new Snake());
                    break;
                case GameMode.SingleAi:
                    var id2 = this.GetSnakeId();
                    this.gameState.Players[id2] = new Player(
                        name: "Ai",
                        snakeId: id2,
                        type: PlayerType.Ai,
                        snake: new SnakeEnеmy());
                    break;
                case GameMode.PlayerVsAi:
                    var pId = this.GetSnakeId();
                    this.gameState.Players[pId] = new Player(
                        name: "Player",
                        snakeId: pId,
                        type: PlayerType.Human,
                        snake: new Snake());
                    var aiId = this.GetSnakeId();
                    this.gameState.Players[aiId] = new Player(
                        name: "AI",
                        snakeId: aiId,
                        type: PlayerType.Ai,
                        snake: new SnakeEnеmy());
                    break;

                case GameMode.AiVsAi:
                    var ai1 = this.GetSnakeId();
                    this.gameState.Players[ai1] = new Player(
                        name: "AI 1",
                        snakeId: ai1,
                        type: PlayerType.Ai,
                        snake: new Snake());

                    var ai2 = this.GetSnakeId();
                    this.gameState.Players[ai2] = new Player(
                        name: "AI 2",
                        snakeId: ai2,
                        type: PlayerType.Ai,
                        snake: new SnakeEnеmy());
                    break;

                case GameMode.PlayerVsPlayer:
                    var p1 = this.GetSnakeId();
                    this.gameState.Players[p1] = new Player(
                        name: "Player 1",
                        snakeId: p1,
                        type: PlayerType.Human,
                        snake: new Snake());

                    var p2 = this.GetSnakeId();
                    this.gameState.Players[p2] = new Player(
                        name: "Player 2",
                        snakeId: p2,
                        type: PlayerType.Human,
                        snake: new SnakeEnеmy());
                    break;
            }
        }

        private Direction ResolveDirection(Player player, Direction lastDirection, Food? food, KeyPressed keyPressed = KeyPressed.None)
        {
            // В бъдеще тук може да вкараме и GameMode, ако ти трябва различно поведение.
            if (player.Type == PlayerType.Human)
            {
                if (keyPressed == KeyPressed.None)
                {
                    return lastDirection;
                }
                var direction = DirectionService.GetByPressedKey(keyPressed);
                return direction;
            }

            if (player.Type == PlayerType.Ai)
            {
                var direction = this.aiController.GetNextDirection(
                    this.gameBoard,
                    player.Snake.HeadPossition,
                    food!.Coordinates,
                    player.Snake.Body);

                return direction;
            }

            return lastDirection;
        }

        private GameState CreateInitialGameState()
        {
            return null!;
        }

        private Food HandleFoodEaten(Food oldFood, ISnake snake)
        {
            snake.Eat();
            return this.UpdateFood(oldFood);
        }

        private Food InitialGame()
        {
            this.gameBoard.CreateBoard();

            this.InitializePlayers();

            foreach (var kvp in this.gameState.Players)
            {
                var snake = kvp.Value.Snake;
                this.gameBoard.Add(snake.Body);
                this.gameState!.Block(snake.Body);

            }

            var food = this.objectFactory.CreateFood(
                this.gameBoard.BoardConfig,
                this.blockList);
            this.gameState!.Block(food.Coordinates);

            this.obstacles = this.objectFactory.CreateObstacles(
                this.obstaclesCount,
                this.gameBoard.BoardConfig,
                this.blockList);

            var obsCoordinates = this.obstacles.Keys as IReadOnlyCollection<Coordinates>;
            this.gameState.Block(obsCoordinates!);

            this.gameBoard.Add(food.Coordinates, CellType.Food);
            this.gameBoard.Add(obsCoordinates!, CellType.Obstacle);


            return food;
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

        private Food UpdateFood(Food oldFood)
        {
            this.gameBoard.RemoveCellType(oldFood.Coordinates);
            this.gameState!.UnBlock(oldFood.Coordinates);

            var newFood = this.objectFactory.CreateFood(this.gameBoard.BoardConfig, blockList);
            this.gameBoard.Add(newFood.Coordinates, CellType.Food);
            this.gameState.Block(newFood.Coordinates);

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
                    this.gameState!.UnBlock(o.Key);
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
            => this.obstacles.ContainsKey(key: nextHead);
    }
}