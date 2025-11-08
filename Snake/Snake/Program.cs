using SnakeGame.Core;
using SnakeGame.GameObjects;
using SnakeGame.Input;

var snake = new Snake(0, 6);
Console.CursorVisible = false;
var gameTime = new GameTime(targetFps: 10);
var colums = Console.WindowWidth;
var rows = Console.WindowHeight;
var boardSize = new Coordinates(rows, colums);
var obstacle = new Coordinates(500, 500);

var foodFactory = new FoodFactory();
var food = foodFactory.GetFood(boardSize, snake.Body);
Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
Console.WriteLine(food.Symbol);

while (true)
{
    var inputReader = new ConsoleInputReader();
    var direction = inputReader.GetInput();
    snake.Move(direction);

    if (snake.NextHeadPossition == food.Coordinates)
    {
        snake.Eat();
        Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
        Console.WriteLine(' ');
        food = foodFactory.GetFood(boardSize, snake.Body);
        Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
        Console.WriteLine(food.Symbol);
    }

    if (food.IsExpired)
    {
        Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
        Console.WriteLine(' ');
        food = foodFactory.GetFood(boardSize, snake.Body);
        Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
        Console.WriteLine(food.Symbol);
    }

    if (snake.WillDie(boardSize, obstacle))
    {
        Console.WriteLine("Game Over");
        Console.WriteLine($"{snake.NextHeadPossition.Row} -- {snake.NextHeadPossition.Col}");
        Console.WriteLine(snake.NextHeadPossition.IsInRange(boardSize.Row, boardSize.Col));
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
