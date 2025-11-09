using SnakeGame.Core;
using SnakeGame.GameObjects;
using SnakeGame.Input;

var snake = new Snake(0, 6);
Console.CursorVisible = false;
var gameTime = new GameTime(targetFps: 13);
var colums = Console.WindowWidth;
var rows = Console.WindowHeight;
var boardSize = new Coordinates(rows, colums);
var obstacle = new Coordinates(500, 500);

var foodFactory = new FoodFactory();
var food = foodFactory.GetFood(boardSize, snake.Body);
Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
Console.WriteLine(food.Symbol);
var score = 0;
while (true)
{
    var inputReader = new ConsoleInputReader();
    var direction = inputReader.GetInput();

    if (snake.NextHeadPossition == food.Coordinates || snake.HeadPossition == food.Coordinates)
    {
        snake.Eat();
        ClearFood();
        food = foodFactory.GetFood(boardSize, snake.Body);
        DrowFood();
        Console.WriteLine($"Score - {score++}");
    }

    snake.Move(direction);

    if (food.IsExpired)
    {
        ClearFood() ;
        food = foodFactory.GetFood(boardSize, snake.Body);
        DrowFood() ;
    }

    if (snake.WillDie(boardSize, obstacle))
    {
        Console.WriteLine("Game Over");
        break;
    }

    DrowSnake();

    var snakeTail = snake.GetTailPossition;
    Console.SetCursorPosition(snakeTail.Col, snakeTail.Row);
    Console.WriteLine(' ');
    gameTime.Tick();
}

void ClearFood()
{
    Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
    Console.WriteLine(' ');
}

void DrowFood()
{
    Console.SetCursorPosition(food.Coordinates.Col, food.Coordinates.Row);
    Console.WriteLine(food.Symbol);
}

void DrowSnake()
{
    foreach (var item in snake.Body)
    {
        Console.SetCursorPosition(item.Col, item.Row);
        Console.WriteLine('*');
    }
}
