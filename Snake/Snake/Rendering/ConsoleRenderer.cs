namespace SnakeGame.Rendering
{
    using System.Collections.Generic;

    using SnakeGame.GameObjects;

    public class ConsoleRenderer : IRenderer
    {
        public void ClearElement(Coordinates element)
        {
            Console.SetCursorPosition(element.Col, element.Row);
            Console.WriteLine(' ');
        }

        public void DrowFood(Coordinates food, char symbol)
        {
            Console.SetCursorPosition(food.Col, food.Row);
            Console.WriteLine(symbol);
        }

        public void DrowSnake(IReadOnlyCollection<Coordinates> snakeBody)
        {
            foreach (var item in snakeBody)
            {
                Console.SetCursorPosition(item.Col, item.Row);
                Console.WriteLine('*');
            }
        }
    }
}
