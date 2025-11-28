namespace SnakeGame.Core.Scenes;

using System.Collections.Generic;

using SnakeGame.Core.Scenes.ENums;
using SnakeGame.Core.Scenes.Interfaces;
using SnakeGame.Core.State;
using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Abstractions.Interfaces;
using SnakeGame.GameObjects.Enums;
using SnakeGame.Rendering;

/// <summary>
/// Main orchestrator of the Snake gameplay loop.
/// Coordinates: input → movement → spawning → board updates → rendering.
/// Uses double-buffer rendering and a block-list to maintain valid spawn positions.
/// </summary>
public class GameplayScene : IGameScene
{
    private int snakeId = 0;

    private CellType[,] currScene;
    private CellType[,] prevScene;
    private readonly GameState gameState;
    private readonly GameMode gameMode;
    private readonly IGameBoard gameBoard;
    private readonly IGameEngine gameEngine;
    private readonly IGameTime gameTime;
    private readonly int obstaclesCount = 2;
    private readonly IObjectFactory objectFactory;
    private readonly IRenderer renderer;

    public GameplayScene(
            IGameTime gameTime,
            IRenderer renderer,
            IObjectFactory objectFactory,
            IGameBoard gameBoard,
            IGameEngine gameEngine,
            GameMode gameMode = GameMode.SinglePlayer)
    {
        this.gameMode = gameMode;
        this.gameTime = gameTime;
        this.objectFactory = objectFactory;
        this.renderer = renderer;
        this.gameBoard = gameBoard;
        this.gameEngine = gameEngine;
        this.gameState = new(this.gameBoard);

        this.SetScenes();

        this.gameBoard.CreateBoard();
        this.InitializePlayers();
        this.AddSnakesToBoard();
        this.CreateInitialFood();
        this.AddInitialObstacles();
    }

    public void Run()
    {
        this.prevScene = (CellType[,])this.gameBoard.GetBoard.Clone();
        this.renderer.Draw(prevScene);

        while (!this.gameState.IsGameOver)
        {
            this.gameTime.Tick();

            this.gameEngine.FixedUpdate(this.gameState, this.gameTime.DeltaTimeSeconds);

            this.SetCurrScene();

            // Render only the diff between previous and current frame (double-buffer drawing).
            this.RenderBoard();

            // Swap buffers (prev ↔ curr) to enable flicker-free differential rendering.
            this.SwapBuffers();
        }
    }

    private void SetScenes()
    {
        var rows = this.gameBoard.BoardConfig.TotalRows;
        var cols = this.gameBoard.BoardConfig.TotalCols;
        this.prevScene = new CellType[rows, cols];
        this.currScene = new CellType[rows, cols];
    }

    private void SwapBuffers()
        => (this.currScene, this.prevScene) = (this.prevScene, this.currScene);

    private void RenderBoard()
        => this.renderer.Draw(this.prevScene, this.currScene);

    private void SetCurrScene() =>
        this.currScene = (CellType[,])this.gameBoard.GetBoard.Clone();

    private void AddSnakesToBoard()
    {
        foreach (var kvp in this.gameState.Players)
        {
            var snake = kvp.Value.Snake;
            this.gameBoard.Add(snake.Body);
            this.gameState.Block(snake.Body);
        }
    }

    private void AddInitialObstacles()
    {
        this.gameState.Obstacles = this.objectFactory.CreateObstacles(
            this.obstaclesCount,
            this.gameBoard.BoardConfig,
            this.gameState.BlockList);
        var obsCoordinates = this.gameState.Obstacles.Keys as IReadOnlyCollection<Coordinates>;
        this.gameState.Block(obsCoordinates!);
        this.gameBoard.Add(obsCoordinates!, CellType.Obstacle);
    }

    private void CreateInitialFood()
    {
        var food = this.objectFactory.CreateFood(
            this.gameBoard.BoardConfig,
            this.gameState.BlockList);
        this.gameState.Food = food;
        this.gameState.Block(food.Coordinates);
        this.gameBoard.Add(food.Coordinates, CellType.Food);
    }

    private SnakeId GetSnakeId()
    {
        return new SnakeId(++this.snakeId);
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
}