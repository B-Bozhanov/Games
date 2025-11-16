namespace SnakeGame.GameObjects
{
    using System;

    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    using static System.Net.Mime.MediaTypeNames;

    public class BfsSnakeAiController : ISnakeAiController
    {
        public Direction GetNextDirection(SnakeAiContext context)
        {
            var path = this.FindPath(context);

            if (path is null || path.Count < 2)
            {
                // fallback – ако няма път, просто продължи по посока
                return Direction.Right;//this.TryNotDie(context); // или context.CurrentDirection, ако го подадем
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

        private Direction TryNotDie(SnakeAiContext context)
        {
            var head = context.Head;
            var board = context.GameBoard;
            var rows = board.BoardConfig.TotalRows;
            var cols = board.BoardConfig.TotalCols;

            var snakeBody = new HashSet<Coordinates>(context.Body);

            var possibleDirections = new[]
            {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right,
            };

            var safeMoves = new List<Direction>();

            // 1) Търсим истински "safe" ходове:
            //   - не умираш веднага
            //   - има поне малко пространство за дишане (HasEscapePath)
            foreach (var dir in possibleDirections)
            {
                var offset = GetOffsetForDirection(dir);
                var next = head + offset;

                if (!next.IsInRange(rows, cols))
                {
                    continue;
                }

                if (this.WillDieImmediate(next, snakeBody, context))
                {
                    continue;
                }

                if (!this.HasEscapePath(next, snakeBody, context))
                {
                    continue;
                }

                safeMoves.Add(dir);
            }

            if (safeMoves.Count > 0)
            {
                return this.ChooseBestSafeMove(safeMoves, head, snakeBody);
            }

            // 2) Ако няма истински safe ходове, поне такъв, който не умира веднага
            foreach (var dir in possibleDirections)
            {
                var offset = GetOffsetForDirection(dir);
                var next = head + offset;

                if (!next.IsInRange(rows, cols))
                {
                    continue;
                }

                if (!this.WillDieImmediate(next, snakeBody, context))
                {
                    return dir;
                }
            }

            // 3) Абсолютен fallback – продължи надясно (или текущата посока, ако я имаш в context)
            // return context.CurrentDirection;
            return Direction.Right;
        }

        private bool WillDieImmediate(
            Coordinates next,
            HashSet<Coordinates> snakeBody,
            SnakeAiContext context)
        {
            var board = context.GameBoard;
            var rows = board.BoardConfig.TotalRows;
            var cols = board.BoardConfig.TotalCols;
            var tail = snakeBody.First();
            bool isTailCell = next == tail;
            bool willEat = (next == context.Food);

            if (!next.IsInRange(rows, cols))
            {
                return true;
            }

            if (snakeBody.Contains(next))
            {
                if (isTailCell && !willEat)
                {
                    return false;
                }
                else
                {
                    return true; // удар в тяло
                }
            }

            var cell = board.GetCellType(next);

            if (cell == CellType.Obstacle)
            {
                return true;
            }

            // ако имаш CellType.Wall, добави:
            // if (cell == CellType.Wall) return true;

            return false;
        }

        private bool HasEscapePath(
            Coordinates start,
            HashSet<Coordinates> snakeBody,
            SnakeAiContext context)
        {
            var board = context.GameBoard;
            var rows = board.BoardConfig.TotalRows;
            var cols = board.BoardConfig.TotalCols;

            var queue = new Queue<Coordinates>();
            var visited = new HashSet<Coordinates>();

            queue.Enqueue(start);
            visited.Add(start);

            int freeCells = 0;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                freeCells++;

                // ако намерим поне няколко свободни клетки – достатъчно е, не сме в капан
                if (freeCells >= 4)
                {
                    return true;
                }

                foreach (var neighbor in GetNeighbors(current, rows, cols))
                {
                    if (visited.Contains(neighbor))
                    {
                        continue;
                    }

                    if (this.IsBlocked(neighbor, snakeBody, context))
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            // няма достатъчно свободно пространство – капан
            return false;
        }

        private bool IsBlocked(
            Coordinates coord,
            HashSet<Coordinates> snakeBody,
            SnakeAiContext context)
        {
            var tail = snakeBody.First();      // assuming Queue front = tail
            var willEat = coord == context.Food;

            if(coord == tail && !willEat)
              return false;

            if (snakeBody.Contains(coord))
            {
                return true;
            }

            var cell = context.GameBoard.GetCellType(coord);

            if (cell == CellType.Obstacle)
            {
                return true;
            }

            // ако имаш CellType.Wall:
            // if (cell == CellType.Wall) return true;

            return false;
        }

        private static IEnumerable<Coordinates> GetNeighbors(
            Coordinates c,
            int rows,
            int cols)
        {
            var directions = new[]
            {
                new Coordinates(-1, 0), // up
                new Coordinates(1, 0),  // down
                new Coordinates(0, -1), // left
                new Coordinates(0, 1),  // right
            };

            foreach (var d in directions)
            {
                var next = c + d;

                if (next.IsInRange(rows, cols))
                {
                    yield return next;
                }
            }
        }

        private Direction ChooseBestSafeMove(
            IEnumerable<Direction> safeMoves,
            Coordinates head,
            HashSet<Coordinates> snakeBody)
        {
            // проста евристика – избери хода, който държи главата
            // най-далеч от останалото тяло (да не се навира навътре)
            return safeMoves
                .OrderByDescending(dir =>
                {
                    var offset = GetOffsetForDirection(dir);
                    var next = head + offset;
                    return DistanceFromBody(next, snakeBody);
                })
                .First();
        }

        private static int DistanceFromBody(Coordinates pos, HashSet<Coordinates> body)
        {
            var min = int.MaxValue;

            foreach (var part in body)
            {
                var d = Math.Abs(part.Row - pos.Row) + Math.Abs(part.Col - pos.Col);

                if (d < min)
                {
                    min = d;
                }
            }

            return min;
        }

        private static Coordinates GetOffsetForDirection(Direction dir)
        {
            return dir switch
            {
                Direction.Up => new Coordinates(-1, 0),
                Direction.Down => new Coordinates(1, 0),
                Direction.Left => new Coordinates(0, -1),
                Direction.Right => new Coordinates(0, 1),
                _ => new Coordinates(0, 0),
            };
        }
    }
}

