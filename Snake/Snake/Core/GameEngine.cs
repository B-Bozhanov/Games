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


            this.gameBoard.Add(food.Coordinates, CellType.Food);
            this.gameBoard.Add(snake.Body, CellType.SnakeBody);


            var score = 1;
            var currentSpeed = 1;
            var prevScene = (CellType[,])this.gameBoard.GetMatrix.Clone();
            this.gameBoard.RenderBoard();
            while (true)
            {
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
                    this.gameBoard.RemoveCellType(food.Coordinates);
                    food = this.foodFactory.GetFood(boardSize, snake.Body);
                    this.gameBoard.Add(food.Coordinates, CellType.Food);
                }

                this.gameBoard.Add(this.snake.Body, CellType.SnakeBody);
                if (!this.snake.ShouldEat)
                {
                    this.gameBoard.RemoveCellType(this.snake.GetLastTailPossition);
                }


                var currentScene = this.gameBoard.GetMatrix;
                this.renderer.Draw(prevScene, currentScene);
                Array.Copy(currentScene, prevScene, prevScene.Length); ; 
                this.gameTime.Tick();
            }
        }
    }
}
