using SnakeGame.Core;
using SnakeGame.GameObjects;
using SnakeGame.Input;
using SnakeGame.Rendering;

var snake = new Snake(0, 6);
Console.CursorVisible = false;
var gameTime = new GameTime(targetFps: 13);
var colums = Console.WindowWidth;
var rows = Console.WindowHeight;
var boardSize = new Coordinates(rows, colums);
var obstacle = new Coordinates(500, 500);

var renderer = new ConsoleRenderer();

var foodFactory = new FoodFactory();
var food = foodFactory.GetFood(boardSize, snake.Body);

renderer.DrowFood(food.Coordinates, food.Symbol);

var score = 0;
while (true)
{
    var inputReader = new ConsoleInputReader();
    var direction = inputReader.GetInput();

    if (snake.NextHeadPossition == food.Coordinates || snake.HeadPossition == food.Coordinates)
    {
        snake.Eat();
        renderer.ClearElement(food.Coordinates);
        food = foodFactory.GetFood(boardSize, snake.Body);
        renderer.DrowFood(food.Coordinates, food.Symbol);

        Console.SetCursorPosition(0, 0);
        Console.Write($"Score - {score++}");
    }

    snake.Move(direction);

    if (food.IsExpired)
    {
        renderer.ClearElement(food.Coordinates);
        food = foodFactory.GetFood(boardSize, snake.Body);
        renderer.DrowFood(food.Coordinates, food.Symbol);
    }

    if (snake.WillDie(boardSize, obstacle))
    {
        Console.WriteLine("Game Over");
        break;
    }

    renderer.DrowSnake(snake.Body);

    renderer.ClearElement(snake.GetTailPossition);

    gameTime.Tick();
}
