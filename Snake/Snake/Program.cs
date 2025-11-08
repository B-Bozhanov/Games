using SnakeGame.Core;
using SnakeGame.GameObjects;
using SnakeGame.Input;

var snake = new Snake(0, 6);
Console.CursorVisible = false;
var gameTime = new GameTime(targetFps: 3);
var width = Console.WindowWidth;
var height = Console.WindowHeight;
var boardSize = new Coordinates(width, height);
var obstacle = new Coordinates(50, 50);
var food = new Food();
while (true)
{
    //snake.Eat();
    var inputReader = new ConsoleInputReader();
    var direction = inputReader.GetInput();
    snake.Move(direction);
    var currentFood = food.Generate(boardSize, snake.Body);
    Console.SetCursorPosition(currentFood.Col, currentFood.Row);
    Console.WriteLine(food.Symbol);
    if (snake.WillDie(boardSize, obstacle))
    {
        Console.WriteLine("Game Over");
        break;
    }

    foreach (var item in snake.Body)
    {
        Console.SetCursorPosition(item.Col, item.Row);
        Console.WriteLine('*');
    }

    var snakeTail = snake.GetTailPossition;
    Console.SetCursorPosition(snakeTail.Col, snakeTail.Row);
    Console.WriteLine(' ');
    gameTime.Tick();
}
