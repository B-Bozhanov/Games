namespace SnakeGame.Core.GameLoop;

using System.Collections.Generic;

using SnakeGame.Core.Controllers;
using SnakeGame.Core.Controllers.Interfaces;
using SnakeGame.Core.GameLoop.Interfaces;
using SnakeGame.Core.State;
using SnakeGame.Extensions;
using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Abstractions.Interfaces;
using SnakeGame.GameObjects.Enums;

public sealed class GameEngine : IGameEngine
{
    private readonly IObjectFactory objectFactory;
    private readonly ISnakeController controller;
    private IDictionary<Coordinates, Obstacle> obstacles;

    public GameEngine(
        IObjectFactory objectFactory,
        ISnakeController controller)
    {
        this.objectFactory = objectFactory;
        this.controller = controller;
        this.obstacles = new Dictionary<Coordinates, Obstacle>();
    }

    public void FixedUpdate(GameState gameState, double deltaSeconds)
    {
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


            var context = new GetNextDirectionsContext
                (
                    GameBoard: gameState.GameBoard,
                    Player: player,
                    LastDirection: player.Snake.CurrentDirection,
                    Food: food,
                    GameState: gameState
                );

            var direction = this.controller.GetNextDirection(context);
            var nextHead = player.Snake.GetNextHeadPossition(direction);

            if (this.WillDie(gameState, nextHead) || player.Snake.WillCollideWithSelf(nextHead))
            {
                player.IsAlive = false;
                gameState.IsGameOver = true;
            }

            Eat(gameState, player, ref food, nextHead);
            if (food.IsExpired)
            {
                food = this.UpdateFood(gameState, food);
            }

            this.UpdateObstacles(gameState);

            UpdateSnake(gameState, direction, player.Snake, nextHead);

            gameState.GameBoard.Add(player.Snake.Body, CellType.SnakeBody);
            gameState.GameBoard.Add(player.Snake.HeadPossition, player.Snake.NextHeadPossitionSymbol);
            gameState.GameBoard.Add(player.Snake.GetCurrentTailPossition, player.Snake.NextTailPossitionSymbol);

            if (!player.Snake.ShouldEat)
            {
                gameState.GameBoard.RemoveCellType(player.Snake.GetLastTailPossition);
            }

            gameState.Food = food;
        }
    }

    private void Eat(GameState gameState, Player player, ref Food food, Coordinates nextHead)
    {
        if (nextHead == food!.Coordinates)
        {
            food = this.HandleFoodEaten(gameState, food, player.Snake);
            if (player.MoveIntervalSeconds > 0.01)
            {
                player.MoveIntervalSeconds -= 0.01;
            }

            player.Score++;
        }
    }

    private Food HandleFoodEaten(GameState gameState, Food oldFood, ISnake snake)
    {
        snake.Eat();
        return this.UpdateFood(gameState, oldFood);
    }

    private Food UpdateFood(GameState gameState, Food oldFood)
    {
        gameState.GameBoard.RemoveCellType(oldFood.Coordinates);
        gameState.UnBlock(oldFood.Coordinates);

        var newFood = this.objectFactory.CreateFood(gameState.BoardConfig, gameState.BlockList);
        gameState.GameBoard.Add(newFood.Coordinates, CellType.Food);
        gameState.Block(newFood.Coordinates);

        return newFood;
    }

    private void UpdateObstacles(GameState gameState)
    {
        var expiredKeys = new List<Coordinates>();

        foreach (var o in gameState.Obstacles)
        {
            if (o.Value.IsExpired)
            {
                gameState.GameBoard.RemoveCellType(o.Key);
                gameState!.UnBlock(o.Key);
                expiredKeys.Add(o.Key);
            }
        }

        if (expiredKeys.Count == 0) return;

        gameState.Obstacles.RemoveRange(expiredKeys);

        var newObstacles = this.objectFactory.CreateObstacles(
            expiredKeys.Count,
            gameState.GameBoard.BoardConfig,
            gameState!.BlockList);

        foreach (var kvp in newObstacles)
        {
            gameState.Obstacles.Add(kvp);
            gameState!.Block(kvp.Key);
            gameState.GameBoard.Add(kvp.Key, CellType.Obstacle);
        }
    }

    private static void UpdateSnake(GameState gameState, Direction direction, ISnake snake, Coordinates nextHead)
    {
        gameState!.UnBlock(snake.Body);
        snake.Move(direction);
        gameState!.Block(snake.Body);
        gameState!.Block(nextHead);
    }

    private bool WillDie(GameState gameState, Coordinates nextHead)
             => WillHitObstacle(gameState, nextHead)
             || !nextHead.IsInRange(gameState.BoardConfig.TotalRows, gameState.BoardConfig.TotalCols);

    private static bool WillHitObstacle(GameState gameState, Coordinates nextHead)
        => gameState.Obstacles.ContainsKey(key: nextHead);
}
