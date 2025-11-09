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

        public GameEngine(IInputReader inputReader, IGameTime gameTime,
                          IRenderer renderer, IFoodFactory foodFactory, ISnake snake)
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


            renderer.DrowFood(food.Coordinates, food.Symbol);

            var score = 0;
            while (true)
            {
                var direction = this.inputReader.GetInput();

                if (this.snake.NextHeadPossition == food.Coordinates || this.snake.HeadPossition == food.Coordinates)
                {
                    this.snake.Eat();
                    this.renderer.ClearElement(food.Coordinates);
                    food = foodFactory.GetFood(boardSize, snake.Body);
                    this.renderer.DrowFood(food.Coordinates, food.Symbol);

                    Console.SetCursorPosition(0, 0);
                    Console.Write($"Score - {score++}");
                }

                this.snake.Move(direction);

                if (food.IsExpired)
                {
                    this.renderer.ClearElement(food.Coordinates);
                    food = this.foodFactory.GetFood(boardSize, snake.Body);
                    this.renderer.DrowFood(food.Coordinates, food.Symbol);
                }

                if (snake.WillDie(boardSize, obstacle))
                {
                    Console.WriteLine("Game Over");
                    break;
                }

                this.renderer.DrowSnake(this.snake.Body);

                this.renderer.ClearElement(this.snake.GetTailPossition);

                this.gameTime.Tick();
            }
        }
    }
}
