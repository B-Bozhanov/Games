using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Enums;

var snake = new Snake(0, 6);

while (true)
{
    snake.Eat();
    snake.Move(Direction.Right);
    foreach (var item in snake.Body)
    {
        Console.SetCursorPosition(item.Col, item.Row);
        Console.WriteLine('*');
    }

    Thread.Sleep(200);

    Console.Clear();
}
