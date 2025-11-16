namespace SnakeGame.GameObjects
{
    using System;

    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public class BfsSnakeAiController : ISnakeAiController
    {
        public Direction GetNextDirection(SnakeAiContext context)
        {
            var path = this.FindPath(context);

            if (path is null || path.Count < 2)
            {
                // fallback – ако няма път, просто продължи по посока
                return Direction.Right; // или context.CurrentDirection, ако го подадем
            }

            var head = context.Head;
            var next = path[1]; // [0] = head, [1] = първа стъпка

            return GetDirectionFromTo(head, next);
        }

        private static Direction GetDirectionFromTo(Coordinates from, Coordinates to)
        {
            var dRow = to.Row - from.Row;
            var dCol = to.Col - from.Col;

            return (dRow, dCol) switch
            {
                (-1, 0) => Direction.Up,
                (1, 0) => Direction.Down,
                (0, -1) => Direction.Left,
                (0, 1) => Direction.Right,
                _ => Direction.Right, // fallback
            };
        }

        private List<Coordinates>? FindPath(SnakeAiContext context)
        {
            var start = context.Head;
            var target = context.Food;

            var board = context.GameBoard;
            var rows = board.BoardConfig.TotalRows;
            var cols = board.BoardConfig.TotalCols;

            var snakeBody = new HashSet<Coordinates>(context.Body);

            var queue = new Queue<Coordinates>();
            var visited = new HashSet<Coordinates>();
            var parent = new Dictionary<Coordinates, Coordinates>();

            queue.Enqueue(start);
            visited.Add(start);

            var directions = new[]
            {
            new Coordinates(-1, 0), // up
            new Coordinates(1, 0),  // down
            new Coordinates(0, -1), // left
            new Coordinates(0, 1),  // right
        };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Equals(target))
                {
                    return this.ReconstructPath(parent, start, target);
                }

                foreach (var dir in directions)
                {
                    var next = current + dir;

                    if (!next.IsInRange(rows, cols))
                    {
                        continue;
                    }

                    // не стъпваме върху змията, стените и obstacle-ите
                    if (snakeBody.Contains(next))
                    {
                        continue;
                    }

                    var cell = board.GetCellType(next); // трябва да имаш такъв метод

                    if (cell == CellType.Obstacle)
                    {
                        continue;
                    }

                    if (visited.Contains(next))
                    {
                        continue;
                    }

                    visited.Add(next);
                    parent[next] = current;
                    queue.Enqueue(next);
                }
            }

            return null; // няма път
        }

        private List<Coordinates>? ReconstructPath(Dictionary<Coordinates, Coordinates> parent, Coordinates start, Coordinates target)
        {
            var path = new List<Coordinates>();
            var current = target;

            while (!current.Equals(start))
            {
                path.Add(current);
                current = parent[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}
