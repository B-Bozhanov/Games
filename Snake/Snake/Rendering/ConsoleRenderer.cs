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
            Console.Write(' ');
        }

        public void Draw(Coordinates possition, string symbol, Color color = Color.None)
        {
            Console.SetCursorPosition(possition.Col, possition.Row);
            Console.Write(symbol);
        }

        public void Draw(IReadOnlyCollection<Coordinates> coordinates)
        {
            foreach (var item in coordinates)
            {
                Console.SetCursorPosition(item.Col, item.Row);
                Console.Write('*');
            }
        }

        public void Draw(IReadOnlyCollection<Coordinates> coordinates, Color color = Color.None)
        {
            throw new NotImplementedException();
        }
    }
}
