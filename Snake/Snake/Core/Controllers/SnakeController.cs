namespace SnakeGame.Core.Controllers
{
    using SnakeGame.Core.Controllers.Interfaces;
    using SnakeGame.GameObjects.Enums;
    using SnakeGame.Input;
    using SnakeGame.Input.Enums;
    using SnakeGame.Services;
    using SnakeGame.SnakeAI;

    public class SnakeController(IInputReader inputReader, ISnakeAiController aiController) : ISnakeController
    {
        private readonly IInputReader inputReader = inputReader;
        private readonly ISnakeAiController aiController = aiController;

        public Direction GetNextDirection(GetNextDirectionsContext context)
        {
            if (context.Player.Type == PlayerType.Human)
            {
                var keyPressed = this.inputReader.GetInput();
                if (keyPressed == KeyPressed.None)
                {
                    return context.LastDirection;
                }

                var direction = DirectionService.GetByPressedKey(keyPressed);
                return direction;
            }

            if (context.Player.Type == PlayerType.Ai)
            {
                var direction = this.aiController.GetNextDirection(
                    context.GameBoard,
                    context.Player.Snake.HeadPossition,
                    context.Food.Coordinates,
                    context.Player.Snake.Body);

                return direction;
            }

            return context.LastDirection;
        }
    }
}