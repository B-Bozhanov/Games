namespace SnakeGame.GameObjects
{
    public class Food
    {
        private readonly Random random;
        private readonly char symbol;

        public Food()
        {
            this.random = new Random();
            this.symbol = '@';
        }

        public Char Symbol => this.symbol;

        public Coordinates Generate(Coordinates boardSize, IReadOnlyCollection<Coordinates> snakeBody)
        {
            while (true)
            {
                int x = this.random.Next(0, boardSize.Row);
                int y = this.random.Next(0, boardSize.Col);

                var food = new Coordinates(x, y);

                bool isOnSnake = snakeBody.Contains(food);
                if (isOnSnake)
                {
                    continue; 
                }

                return food;
            }
        }
    }
}
