namespace SnakeGame.Core
{
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.GameObjects.Interfaces;
    using SnakeGame.Input;
    using SnakeGame.Rendering;

    public class GameEngine(
        IInputReader inputReader,
        IGameTime gameTime,
        IRenderer renderer,
        IFoodFactory foodFactory,
        ISnake snake,
        IGameBoard gameBoard) : IGameScene
    {

        private readonly IInputReader inputReader = inputReader;
        private readonly IGameTime gameTime = gameTime;
        private readonly IRenderer renderer = renderer;
        private readonly IFoodFactory foodFactory = foodFactory;
        private readonly ISnake snake = snake;
        private readonly IGameBoard gameBoard = gameBoard;

        public void Run()
        {
            Console.CursorVisible = false;
            var colums = Console.WindowWidth;
            var rows = Console.WindowHeight;
            var boardSize = new Coordinates(rows, colums);
            var food = this.foodFactory.GetFood(boardSize, snake.Body);
            var obstacle = new Coordinates(500, 500);

            this.gameBoard.CreateBoarder();
            this.gameBoard.RenderBoard();


            this.renderer.Draw(food.Coordinates, food.Coordinates.Symbol.ToString());

            var score = 1;
            var currentSpeed = 1;
            while (true)
            {
                Console.SetCursorPosition(50, 1);
                Console.Write($"Speed -- {currentSpeed}");
                var direction = this.inputReader.GetInput();
                var nexHead = snake.GetNextHeadPossition(direction);

                if (snake.WillDie(boardSize, obstacle, direction))
                {
                    Console.Write("Game Over");
                    break;
                }

                this.snake.Move(direction);

                if (nexHead == food.Coordinates)
                {
                    this.snake.Eat();
                    this.gameBoard.RemoveCellType(food.Coordinates);
                    food = foodFactory.GetFood(boardSize, snake.Body);
                    this.gameBoard.Add(food.Coordinates, CellType.Food);
                    this.gameTime.IncreaseSpeed();
                    currentSpeed++;
                }

                if (food.IsExpired)
                {
                    this.renderer.ClearElement(food.Coordinates);
                    food = this.foodFactory.GetFood(boardSize, snake.Body);
                    this.renderer.Draw(food.Coordinates, food.Coordinates.Symbol.ToString());
                }

                this.gameBoard.Add(this.snake.Body, CellType.SnakeBody);
                //this.renderer.Draw(food);
                if (!this.snake.ShouldEat)
                {
                    this.renderer.ClearElement(this.snake.GetLastTailPossition);
                }

                this.gameTime.Tick();
            }
        }
    }
}
