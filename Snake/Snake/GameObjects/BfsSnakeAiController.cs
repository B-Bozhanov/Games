namespace SnakeGame.GameObjects
{
    using System;

    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    using static System.Net.Mime.MediaTypeNames;

    public class BfsSnakeAiController : ISnakeAiController
    {
        private int count = 0;
        private readonly List<Coordinates> hamiltonianCycle = new();
        private readonly Dictionary<Coordinates, int> coordIndex = new();

        private bool hamiltonianInitialized;
        private const int HamiltonianThreshold = 80; // над колко дължина да минава в този режим


        public Direction GetNextDirection(SnakeAiContext context)
        {
            var board = context.GameBoard;
            var rows = board.BoardConfig.TotalRows;
            var cols = board.BoardConfig.TotalCols;

            var head = context.Head;
            var food = context.Food;
            var snakeBody = new HashSet<Coordinates>(context.Body);

            // 1) Опитваме път до храната с BFS (само като hint)
            var path = this.FindPath(context);

            Coordinates? bfsNextStep = null;
            bool hasBfsNextStep = path is { Count: >= 2 };

            if (hasBfsNextStep)
            {
                bfsNextStep = path![1]; // [0] = head, [1] = първата стъпка след главата
            }

            var possibleDirections = new[]
            {
        Direction.Up,
        Direction.Down,
        Direction.Left,
        Direction.Right,
    };

            // 2) Оценяваме ВСИЧКИ валидни ходове:
            //    - да НЕ умират веднага (WillDieImmediate)
            //    - площ (reachable area)
            //    - разстояние до храната
            //    - дали следват BFS стъпката
            var scoredMoves = new List<(Direction Dir, int Area, int DistanceToFood, bool FollowsBfs)>();

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

                int area = this.ComputeReachableAreaSize(next, snakeBody, context);

                int distToFood = ManhattanDistance(next, food);

                bool followsBfs =
                    hasBfsNextStep &&
                    bfsNextStep.HasValue &&
                    next.Equals(bfsNextStep.Value);

                scoredMoves.Add((dir, area, distToFood, followsBfs));
            }

            // 3) Ако имаме поне един разумен ход – избираме
            if (scoredMoves.Count > 0)
            {
                // най-голямата налична площ
                int maxArea = scoredMoves.Max(x => x.Area);

                // 3.1. Първо филтрираме само "широките" ходове
                //     (например >= 70% от най-голямата площ)
                var wideMoves = scoredMoves
                    .Where(x => x.Area >= maxArea * 70 / 100)
                    .ToList();

                if (wideMoves.Count == 0)
                {
                    wideMoves = scoredMoves; // ако всички са тесни, работим с каквото има
                }

                // 3.2. Ако сред широките има ход, който следва BFS – предпочитаме него
                var wideBfsMoves = wideMoves
                    .Where(x => x.FollowsBfs)
                    .ToList();

                if (wideBfsMoves.Count > 0)
                {
                    // от BFS ходовете, които са достатъчно широки,
                    // взимаме този, който е най-близо до храната
                    return wideBfsMoves
                        .OrderBy(x => x.DistanceToFood)
                        .First()
                        .Dir;
                }

                // 3.3. Иначе – от всички широки ходове взимаме този,
                //      който е най-близо до храната
                return wideMoves
                    .OrderBy(x => x.DistanceToFood)
                    .First()
                    .Dir;
            }

            // 4) Ако НЯМА нито един ход с positive area (наистина кофти),
            //    поне търсим посока, в която не умираме веднага.
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

            // 5) Абсолютно безизходна ситуация – тук каквото и да върнеш, си свършен.
            // Ако имаш context.CurrentDirection – по-добре е да я върнеш.
            // return context.CurrentDirection;
            return Direction.Right;
        }

        private static int ManhattanDistance(Coordinates a, Coordinates b)
        {
            return Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);
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

            // (direction, reachableAreaSize)
            var scoredMoves = new List<(Direction Dir, int Area)>();

            foreach (var dir in possibleDirections)
            {
                var offset = GetOffsetForDirection(dir);
                var next = head + offset;

                // извън борда?
                if (!next.IsInRange(rows, cols))
                {
                    continue;
                }

                // умира ли веднага (тяло/obstacle/стена)?
                if (this.WillDieImmediate(next, snakeBody, context))
                {
                    continue;
                }

                // колко свободно пространство има около този ход?
                var area = this.ComputeReachableAreaSize(next, snakeBody, context);

                scoredMoves.Add((dir, area));
            }

            // 1) Ако имаме поне една посока с някакво свободно пространство → взимаме тази с най-голямата площ
            if (scoredMoves.Count > 0)
            {
                return scoredMoves
                    .OrderByDescending(x => x.Area)
                    .First()
                    .Dir;
            }

            // 2) Ако flood-fill нищо не е намерил (много зле е положението),
            //    поне избери посока, в която не умираш веднага.
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

            // 3) Абсолютен fallback – ако имаш текуща посока в контекста, ползвай нея.
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

            if (!next.IsInRange(rows, cols))
            {
                return true;
            }

            // tail logic:
            // позволяваме да влезем в клетката на опашката,
            // ако няма да ядем – опашката ще се премести в същия тик
            var tail = snakeBody.First(); // assuming Queue front = tail
            bool isTailCell = next == tail;
            bool willEat = next == context.Food;

            if (snakeBody.Contains(next))
            {
                if (isTailCell && !willEat)
                {
                    // безопасно – опашката ще се отдръпне
                    return false;
                }

                // удар в тяло
                return true;
            }

            var cell = board.GetCellType(next);

            if (cell == CellType.Obstacle)
            {
                return true;
            }

            // ако имаш стени като CellType.Wall, добави:
            // if (cell == CellType.Wall) return true;

            return false;
        }

        private bool IsBlocked(
            Coordinates coord,
            HashSet<Coordinates> snakeBody,
            SnakeAiContext context)
        {
            var tail = snakeBody.First();      // assuming Queue front = tail
            bool willEat = coord == context.Food;

            // tail logic и тук – за BFS/escape check и т.н.
            if (coord == tail && !willEat)
            {
                return false;
            }

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

        /// <summary>
        /// Колко свободни клетки са достижими от дадена позиция (flood-fill / BFS).
        /// Използва по-консервативен block (тялото се брои за стена).
        /// </summary>
        private int ComputeReachableAreaSize(
            Coordinates start,
            HashSet<Coordinates> snakeBody,
            SnakeAiContext context)
        {
            var board = context.GameBoard;
            var rows = board.BoardConfig.TotalRows;
            var cols = board.BoardConfig.TotalCols;

            var visited = new HashSet<Coordinates>();
            var queue = new Queue<Coordinates>();

            visited.Add(start);
            queue.Enqueue(start);

            int count = 0;

            // по желание лимит да не обхождаш цялата карта при много големи boards
            const int MaxCells = 4000;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                count++;

                if (count >= MaxCells)
                {
                    break;
                }

                foreach (var neighbor in GetNeighbors(current, rows, cols))
                {
                    if (visited.Contains(neighbor))
                    {
                        continue;
                    }

                    if (this.IsBlockedForAreaCheck(neighbor, snakeBody, context))
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return count;
        }

        /// <summary>
        /// Block логика за flood-fill:
        /// - по-консервативна от WillDieImmediate
        /// - тялото се брои за стена (няма tail logic тук, нарочно)
        /// </summary>
        private bool IsBlockedForAreaCheck(
            Coordinates coord,
            HashSet<Coordinates> snakeBody,
            SnakeAiContext context)
        {
            if (snakeBody.Contains(coord))
            {
                return true;
            }

            var cell = context.GameBoard.GetCellType(coord);

            if (cell == CellType.Obstacle)
            {
                return true;
            }

            // ако имаш стени:
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

        private static Coordinates GetOffsetForDirection(Direction dir)
        {
            return dir switch
            {
                Direction.Up => new Coordinates(-1, 0),
                Direction.Down => new Coordinates(1, 0),
                Direction.Left => new Coordinates(0, -1),
                Direction.Right => new Coordinates(0, 1), // ако долу ти е +1
                _ => new Coordinates(0, 0),
            };
        }
    }
}