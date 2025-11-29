namespace SnakeGame.Core.Controllers;

using SnakeGame.Core.State;
using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Abstractions.Interfaces;
using SnakeGame.GameObjects.Enums;

public record GetNextDirectionsContext(
    IGameBoard GameBoard,
    Player Player,
    Direction LastDirection,
    Food Food,
    GameState GameState
);