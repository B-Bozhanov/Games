namespace SnakeGame.Core
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Interfaces;
    using SnakeGame.Input;
    using SnakeGame.Rendering;

    public class GameEngine : IGameScene
    {

        private readonly IInputReader inputReader;
        private readonly IGameTime gameTime;
        private readonly IRenderer renderer;
        private readonly IFoodFactory foodFactory;
        private readonly ISnake snake;

        public GameEngine(
            IInputReader inputReader,
            IGameTime gameTime,
            IRenderer renderer,
            IFoodFactory foodFactory,
            ISnake snake)
        {
            this.inputReader = inputReader;
            this.gameTime = gameTime;
            this.renderer = renderer;
            this.foodFactory = foodFactory;
            this.snake = snake;
        }

        public void Run()
        {
            Console.CursorVisible = false;
            var colums = Console.WindowWidth;
            var rows = Console.WindowHeight;
            var boardSize = new Coordinates(rows, colums);
            var food = this.foodFactory.GetFood(boardSize, snake.Body);
            var obstacle = new Coordinates(500, 500);
            var test = new ConsoleGameBoard();
            test.CreateWalls();


            renderer.Drow(food.Coordinates, food.Symbol);

            var score = 1;
            while (true)
            {
                Console.SetCursorPosition(50, 1);
                Console.WriteLine($"{snake.HeadPossition.Row} ===== {snake.HeadPossition.Col}");
                var direction = this.inputReader.GetInput();
                var nexHead = snake.GetNextHeadPossition(direction);

                if (snake.WillDie(boardSize, obstacle, direction))
                {
                    Console.WriteLine("Game Over");
                    break;
                }

                this.snake.Move(direction);

                if (nexHead == food.Coordinates)
                {
                    this.snake.Eat();
                    this.renderer.ClearElement(food.Coordinates);
                    food = foodFactory.GetFood(boardSize, snake.Body);
                    this.renderer.Drow(food.Coordinates, food.Symbol);

                    test.Score(score++);
                }

                if (food.IsExpired)
                {
                    this.renderer.ClearElement(food.Coordinates);
                    food = this.foodFactory.GetFood(boardSize, snake.Body);
                    this.renderer.Drow(food.Coordinates, food.Symbol);
                }

                this.renderer.Drow(this.snake.Body);
                if (!this.snake.ShouldEat)
                {
                    this.renderer.ClearElement(this.snake.GetLastTailPossition);
                }

                this.gameTime.Tick();
            }
        }
    }
}
