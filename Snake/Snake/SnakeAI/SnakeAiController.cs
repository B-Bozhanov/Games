namespace SnakeGame.SnakeAI
{
    using System.Collections.Generic;

    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.Services;

    // using SnakeGame.Services.Logging; // ако AiLogger ти е в друг namespace

    public sealed class SnakeAiController : ISnakeAiController
    {
        // --------- BFS state ---------
        private Coordinates?[,] parent;

        private readonly HashSet<Coordinates> visited = new();
        private readonly Queue<Coordinates> queue = new();

        // Тялото на змията без главата (само координати на сегментите)
        private readonly HashSet<Coordinates> snakeBody = new();

        // --------- Hamilton state ---------
        private readonly List<Coordinates> hamiltonPath = new();

        private readonly Dictionary<Coordinates, int> hamiltonIndex = new();
        private bool hamiltonInitialized;
        private int hRows;
        private int hCols;

        // Tick брояч (полезен за лог)
        private int tick;

        public Direction GetNextDirection(
            IGameBoard gameBoard,
            Coordinates head,
            Coordinates food,
            IReadOnlyCollection<Coordinates> body)
        {
            this.tick++;

            var rows = gameBoard.BoardConfig.TotalRows;
            var cols = gameBoard.BoardConfig.TotalCols;

            // 1) Подготвяме snakeBody и опашката
            this.snakeBody.Clear();
            Coordinates tail = head;
            bool hasTail = false;

            foreach (var part in body)
            {
                this.snakeBody.Add(part);
                tail = part;           // приемаме, че последният итератор е опашката
                hasTail = true;
            }

            if (!hasTail)
            {
                tail = head;
            }

            // 2) Подготвяме Hamilton пътя
            this.EnsureHamilton(rows, cols);

            Direction chosenDirection = Direction.Right;
            string branch = "Fallback";
            bool hasChoice = false;

            // 3) BFS до храната + tail-safe проверка
            if (this.TryGetBfsDirection(
                    gameBoard,
                    head,
                    food,
                    rows,
                    cols,
                    tail,
                    out var bfsDir,
                    out var bfsNextHead))
            {
                if (this.CanReachTailAfterMove(
                        bfsNextHead,
                        gameBoard,
                        rows,
                        cols,
                        tail))
                {
                    chosenDirection = bfsDir;
                    branch = "BFS";
                    hasChoice = true;
                }
            }

            // 4) Ако BFS не даде безопасен ход → TryNotDie
            if (!hasChoice)
            {
                var survivalDir = this.TryNotDie(
                    gameBoard,
                    head,
                    rows,
                    cols,
                    tail,
                    body.Count + 1);

                if (survivalDir.HasValue)
                {
                    chosenDirection = survivalDir.Value;
                    branch = "TryNotDie";
                    hasChoice = true;
                }
            }

            // 5) Ако още нямаме ход → Hamilton fallback
            if (!hasChoice)
            {
                var hDir = this.GetHamiltonDirection(
                    gameBoard,
                    head,
                    rows,
                    cols,
                    tail);

                if (hDir.HasValue)
                {
                    chosenDirection = hDir.Value;
                    branch = "Hamilton";
                    hasChoice = true;
                }
            }

            // 6) Абсолютен fallback – ако си напълно затворен,
            //    няма да има значение какъв ход правиш
            var finalNextHead = head.Move(chosenDirection);

            // Ако искаш лог – разкоментирай:
            AiLogger.LogDecision(
                this.tick,
                head,
                food,
                body.Count,
                branch,
                chosenDirection,
                finalNextHead,
                "");

            return chosenDirection;
        }

        // =====================================================================
        //  BFS до храната (избягва тялото, допуска само опашката)
        // =====================================================================

        private void ResetBfs(int rows, int cols)
        {
            this.visited.Clear();
            this.queue.Clear();

            if (this.parent == null ||
                this.parent.GetLength(0) != rows ||
                this.parent.GetLength(1) != cols)
            {
                this.parent = new Coordinates?[rows, cols];
            }
            else
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        this.parent[r, c] = null;
                    }
                }
            }
        }

        private bool TryGetBfsDirection(
            IGameBoard board,
            Coordinates head,
            Coordinates food,
            int rows,
            int cols,
            Coordinates tail,
            out Direction direction,
            out Coordinates nextHead)
        {
            direction = Direction.Right;
            nextHead = head;

            this.ResetBfs(rows, cols);

            var start = head;
            this.queue.Enqueue(start);
            this.visited.Add(start);

            while (this.queue.Count > 0)
            {
                var current = this.queue.Dequeue();

                if (current == food)
                {
                    // Възстановяване на пътя: food -> ... -> head
                    var path = new List<Coordinates>();
                    var cur = food;

                    while (!cur.Equals(start))
                    {
                        path.Add(cur);
                        cur = this.parent[cur.Row, cur.Col]!.Value;
                    }

                    path.Add(start);
                    path.Reverse();

                    if (path.Count < 2)
                    {
                        return false;
                    }

                    // path[0] = head, path[1] = първа стъпка
                    var stepDelta = path[1] - start;
                    direction = DirectionService.Get(stepDelta);
                    nextHead = start.Move(direction);
                    return true;
                }

                foreach (var dir in DirectionService.GetAll())
                {
                    var np = current.Move(dir);

                    if (!np.IsInRange(rows, cols))
                    {
                        continue;
                    }

                    var cell = board.GetCellType(np);
                    if (cell == CellType.Obstacle)
                    {
                        continue;
                    }

                    bool isBodyCell = this.snakeBody.Contains(np);
                    bool isTail = np.Equals(tail);

                    // Блокирай всички части от тялото, освен самата опашка
                    if (isBodyCell && !isTail)
                    {
                        continue;
                    }

                    if (this.visited.Contains(np))
                    {
                        continue;
                    }

                    this.queue.Enqueue(np);
                    this.visited.Add(np);

                    if (this.parent[np.Row, np.Col] == null)
                    {
                        this.parent[np.Row, np.Col] = current;
                    }
                }
            }

            return false;
        }

        // =====================================================================
        //  Tail-safe: след хода ще можем ли да стигнем до опашката?
        // =====================================================================

        private bool CanReachTailAfterMove(
            Coordinates nextHead,
            IGameBoard board,
            int rows,
            int cols,
            Coordinates tail)
        {
            var visitedLocal = new HashSet<Coordinates>();
            var q = new Queue<Coordinates>();

            q.Enqueue(nextHead);
            visitedLocal.Add(nextHead);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();

                if (cur.Equals(tail))
                {
                    return true;
                }

                foreach (var dir in DirectionService.GetAll())
                {
                    var np = cur.Move(dir);

                    if (!np.IsInRange(rows, cols))
                    {
                        continue;
                    }

                    var cell = board.GetCellType(np);
                    if (cell == CellType.Obstacle)
                    {
                        continue;
                    }

                    bool isBodyCell = this.snakeBody.Contains(np);
                    bool isTail = np.Equals(tail);

                    if (isBodyCell && !isTail)
                    {
                        continue;
                    }

                    if (visitedLocal.Contains(np))
                    {
                        continue;
                    }

                    visitedLocal.Add(np);
                    q.Enqueue(np);
                }
            }

            return false;
        }

        // =====================================================================
        //  TryNotDie – локално оцеляване + escape пространство
        // =====================================================================

        private Direction? TryNotDie(
            IGameBoard board,
            Coordinates head,
            int rows,
            int cols,
            Coordinates tail,
            int snakeLength)
        {
            var safeMoves = new List<(Direction dir, int space, int hIndex)>();

            foreach (var dir in DirectionService.GetAll())
            {
                var np = head.Move(dir);

                if (!np.IsInRange(rows, cols))
                {
                    continue;
                }

                var cell = board.GetCellType(np);
                if (cell == CellType.Obstacle)
                {
                    continue;
                }

                bool isBodyCell = this.snakeBody.Contains(np);
                bool isTail = np.Equals(tail);

                // Не удряй тялото (освен опашката)
                if (isBodyCell && !isTail)
                {
                    continue;
                }

                // Escape пространство – колко свободни клетки има след този ход
                int space = this.GetEscapeSpace(np, board, rows, cols, tail, snakeLength);
                if (space <= 0)
                {
                    continue;
                }

                int hIndex = this.GetHamiltonIndex(np);
                safeMoves.Add((dir, space, hIndex));
            }

            if (safeMoves.Count > 0)
            {
                // 1) най-много пространство
                // 2) ако са равни – по-напред по Hamilton
                safeMoves.Sort((a, b) =>
                {
                    int cmp = b.space.CompareTo(a.space);
                    if (cmp != 0)
                    {
                        return cmp;
                    }

                    return b.hIndex.CompareTo(a.hIndex);
                });

                return safeMoves[0].dir;
            }

            // Няма „идеални“ safe ходове → търсим поне такъв, който не е моментална смърт
            foreach (var dir in DirectionService.GetAll())
            {
                var np = head.Move(dir);

                if (!np.IsInRange(rows, cols))
                {
                    continue;
                }

                var cell = board.GetCellType(np);
                if (cell == CellType.Obstacle)
                {
                    continue;
                }

                bool isBodyCell = this.snakeBody.Contains(np);
                bool isTail = np.Equals(tail);

                if (isBodyCell && !isTail)
                {
                    continue;
                }

                return dir;
            }

            // Тук реално няма свободна клетка – каквото и да направиш, умираш
            return null;
        }

        private int GetEscapeSpace(
            Coordinates start,
            IGameBoard board,
            int rows,
            int cols,
            Coordinates tail,
            int snakeLength)
        {
            var visitedLocal = new HashSet<Coordinates>();
            var q = new Queue<Coordinates>();

            q.Enqueue(start);
            visitedLocal.Add(start);

            int count = 0;
            // динамичен праг – не е нужно да броим до безкрай
            int maxCount = System.Math.Min(rows * cols, snakeLength * 4);

            while (q.Count > 0 && count < maxCount)
            {
                var cur = q.Dequeue();
                count++;

                foreach (var dir in DirectionService.GetAll())
                {
                    var np = cur.Move(dir);

                    if (!np.IsInRange(rows, cols))
                    {
                        continue;
                    }

                    var cell = board.GetCellType(np);
                    if (cell == CellType.Obstacle)
                    {
                        continue;
                    }

                    bool isBodyCell = this.snakeBody.Contains(np);
                    bool isTail = np.Equals(tail);

                    if (isBodyCell && !isTail)
                    {
                        continue;
                    }

                    if (visitedLocal.Contains(np))
                    {
                        continue;
                    }

                    visitedLocal.Add(np);
                    q.Enqueue(np);
                }
            }

            return count;
        }

        // =====================================================================
        //  Hamilton – серпентина по редовете (глобален ред)
        // =====================================================================

        private void EnsureHamilton(int rows, int cols)
        {
            if (this.hamiltonInitialized &&
                rows == this.hRows &&
                cols == this.hCols)
            {
                return;
            }

            this.hRows = rows;
            this.hCols = cols;
            this.hamiltonPath.Clear();
            this.hamiltonIndex.Clear();

            for (int r = 0; r < rows; r++)
            {
                if (r % 2 == 0)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        var coord = new Coordinates(r, c);
                        this.hamiltonIndex[coord] = this.hamiltonPath.Count;
                        this.hamiltonPath.Add(coord);
                    }
                }
                else
                {
                    for (int c = cols - 1; c >= 0; c--)
                    {
                        var coord = new Coordinates(r, c);
                        this.hamiltonIndex[coord] = this.hamiltonPath.Count;
                        this.hamiltonPath.Add(coord);
                    }
                }
            }

            this.hamiltonInitialized = true;
        }

        private int GetHamiltonIndex(Coordinates coord)
        {
            return this.hamiltonIndex.TryGetValue(coord, out var idx)
                ? idx
                : -1;
        }

        private Direction? GetHamiltonDirection(
            IGameBoard board,
            Coordinates head,
            int rows,
            int cols,
            Coordinates tail)
        {
            if (!this.hamiltonInitialized || this.hamiltonPath.Count == 0)
            {
                return null;
            }

            if (!this.hamiltonIndex.TryGetValue(head, out var headIndex))
            {
                headIndex = this.FindNearestHamiltonIndex(head);
            }

            int snakeLengthApprox = this.snakeBody.Count + 1;
            int maxLookAhead = System.Math.Min(this.hamiltonPath.Count, snakeLengthApprox * 3);

            for (int step = 1; step <= maxLookAhead; step++)
            {
                int idx = headIndex + step;
                if (idx >= this.hamiltonPath.Count)
                {
                    break;
                }

                var target = this.hamiltonPath[idx];

                var dr = target.Row - head.Row;
                var dc = target.Col - head.Col;

                // трябва да е съседна клетка
                if (System.Math.Abs(dr) + System.Math.Abs(dc) != 1)
                {
                    continue;
                }

                if (!target.IsInRange(rows, cols))
                {
                    continue;
                }

                var cell = board.GetCellType(target);
                if (cell == CellType.Obstacle)
                {
                    continue;
                }

                bool isBodyCell = this.snakeBody.Contains(target);
                bool isTail = target.Equals(tail);

                if (isBodyCell && !isTail)
                {
                    continue;
                }

                var delta = target - head;
                var dir = DirectionService.Get(delta);
                return dir;
            }

            return null;
        }

        private int FindNearestHamiltonIndex(Coordinates pos)
        {
            var bestIndex = 0;
            var bestDist = int.MaxValue;

            for (int i = 0; i < this.hamiltonPath.Count; i++)
            {
                var c = this.hamiltonPath[i];
                var dist = System.Math.Abs(c.Row - pos.Row) + System.Math.Abs(c.Col - pos.Col);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
    }
}