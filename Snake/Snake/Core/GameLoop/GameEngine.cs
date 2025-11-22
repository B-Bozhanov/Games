namespace SnakeGame.Core.GameLoop
{
    using System.Collections.Generic;

    using SnakeGame.Core.GameLoop.Interfaces;
    using SnakeGame.Core.State;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.GameObjects.Enums;

    public sealed class GameEngine : IGameEngine
    {
        private readonly IObjectFactory objectFactory;

        public GameEngine(IObjectFactory objectFactory)
        {
            this.objectFactory = objectFactory;
        }

        public void FixedUpdate(
            GameState state,
            IReadOnlyDictionary<SnakeId, Direction> decisions,
            double deltaSeconds)
        {
            if (state.IsGameOver)
            {
                return;
            }

            // 1) Обновяваме "живота" на храна и препятствия
            this.UpdateFood(state, deltaSeconds);
            this.UpdateObstacles(state, deltaSeconds);

            // 2) Движим всяка жива змия според подадените посоки
            foreach (var kvp in state.Snakes)
            {
                var snakeId = kvp.Key;
                var snakeState = kvp.Value;

                if (!snakeState.IsAlive)
                {
                    continue;
                }

                // 2.1) Избираме посока: или от decisions, или текущата
                var direction = snakeState.CurrentDirection;

                if (decisions != null && decisions.TryGetValue(snakeId, out var newDirection))
                {
                    // TODO: по-късно можем да добавим проверка да не обръща на 180 градуса
                    direction = newDirection;
                    snakeState.CurrentDirection = direction;
                }

                // 2.2) Намираме следваща позиция на главата
                var nextHead = this.GetNextHead(snakeState, direction);

                // 2.3) Проверяваме дали ще умре
                if (this.WillHitWall(state, nextHead)
                    || this.WillHitSelf(snakeState, nextHead)
                    || this.WillHitObstacle(state, nextHead))
                {
                    snakeState.IsAlive = false;
                    state.IsGameOver = true;
                    // TODO: по-късно – логика за WinnerSnakeId
                    continue;
                }

                // 2.4) Проверяваме дали яде храна
                var isEating = state.Food is not null
                               && state.Food.Position.Row == nextHead.Row
                               && state.Food.Position.Col == nextHead.Col;

                // 2.5) Move: махаме опашката (ако не яде) и добавяме нова глава
                this.MoveSnake(state, snakeState, nextHead, isEating);

                if (isEating)
                {
                    snakeState.Score++;
                    // Зачиства храна – нова ще се spawn-не по-надолу
                    state.Food = null;
                }
            }

            // 3) Ако няма храна – spawn-нем нова
            if (state.Food is null)
            {
                this.SpawnFood(state);
            }

            // 4) Тик брояч – за статистика / AI
            state.TickCount++;
        }
    }
}
