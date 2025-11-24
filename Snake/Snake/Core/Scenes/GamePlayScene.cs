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
        private readonly IGameEngine gameEngine;

        // blockList keeps track of occupied cells (snake, food, obstacles).
        // Used by factories to guarantee valid spawn positions without scanning the board.
        private readonly bool[,] blockList;

        private CellType[,] prevScene;
        private CellType[,] currScene;
        private int currentSpeed = 1;

        public GameplayScene(
                IInputReader inputReader,
                IGameTime gameTime,
                IRenderer renderer,
                IObjectFactory objectFactory,
                IGameBoard gameBoard,
                ISnakeAiController aiController,
                IGameEngine gameEngine,
                GameMode gameMode = GameMode.SingleAi)
        {
            this.gameMode = gameMode;
            this.inputReader = inputReader;
            this.gameTime = gameTime;
            this.renderer = renderer;
            this.objectFactory = objectFactory;
            this.gameBoard = gameBoard;
            this.aiController = aiController;
            this.gameEngine = gameEngine;
            var rows = this.gameBoard.BoardConfig.TotalRows;
            var cols = this.gameBoard.BoardConfig.TotalCols;
            this.blockList = new bool[rows, cols];
            this.prevScene = new CellType[rows, cols];
            this.currScene = new CellType[rows, cols];

            this.gameState = new(this.gameBoard.BoardConfig);
        }

        public void Run()
        {
            var food = this.InitialGame();

            this.gameState!.Food = food;

            this.prevScene = (CellType[,])this.gameBoard.GetBoard.Clone();
            this.renderer.Draw(prevScene);

            while (!this.gameState.IsGameOver)
            {
                this.gameTime.Tick();
                var test = this.GetDecisions();
                this.gameEngine.FixedUpdate(this.gameState, this.GetDecisions(), this.gameTime.DeltaTimeSeconds);


                Console.SetCursorPosition(3, 2);
                Console.Write($"Fps = {gameTime.CurrentFps}");


                this.currScene = (CellType[,])this.gameBoard.GetBoard.Clone();

                // Render only the diff between previous and current frame (double-buffer drawing).
                this.renderer.Draw(prevScene, currScene);

                // Swap buffers (prev ↔ curr) to enable flicker-free differential rendering.
                (this.currScene, this.prevScene) = (this.prevScene, this.currScene);
            }
        }

        private Dictionary<SnakeId, Direction> GetDecisions()
        {
            var players = this.gameState.Players;
            var dicisions = new Dictionary<SnakeId, Direction>();

            foreach (var (id, player) in players)
            {
                //if (player.Type == PlayerType.Human)
                //{
                //    if (player.MoveTimer < player.MoveIntervalSeconds)
                //    {
                //        continue;
                //    }

                //    var direction = this.ResolveDirection(
                //        player,
                //        player.Snake.CurrentDirection,
                //        this.gameState.Food);

                //    dicisions[id] = direction;
                //}

                dicisions[id] = this.ResolveDirection(
                    player,
                    player.Snake.CurrentDirection,
                    this.gameState.Food);
            }

            return dicisions;
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

                if (player.MoveTimer >= player.MoveIntervalSeconds)
                {
                    keyPressed = this.inputReader.GetInput();
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

            this.gameState.Obstacles = this.objectFactory.CreateObstacles(
                this.obstaclesCount,
                this.gameBoard.BoardConfig,
                this.blockList);

            var obsCoordinates = this.gameState.Obstacles.Keys as IReadOnlyCollection<Coordinates>;
            this.gameState.Block(obsCoordinates!);

            this.gameBoard.Add(food.Coordinates, CellType.Food);
            this.gameBoard.Add(obsCoordinates!, CellType.Obstacle);


            return food;
        }
    }
}