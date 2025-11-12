namespace SnakeGame.Rendering
{
    using System.Collections.Generic;

    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;

    public class ConsoleRenderer : IRenderer
    {
        public void ClearAll() => Console.Clear();

        public void ClearElement(Coordinates element)
        {
            Console.SetCursorPosition(element.Col, element.Row);
            Console.WriteLine(' ');
        }

        public void Drow(Coordinates possition, char symbol, Color color = Color.None)
        {
            Console.SetCursorPosition(possition.Col, possition.Row);
            Console.WriteLine(symbol);
        }

        public void Drow(IReadOnlyCollection<Coordinates> coordinates)
        {
            foreach (var item in coordinates)
            {
                Console.SetCursorPosition(item.Col, item.Row);
                Console.WriteLine('*');
            }
        }

        public void Drow(IReadOnlyCollection<Coordinates> coordinates, Color color = Color.None)
        {
            throw new NotImplementedException();
        }
    }
}
