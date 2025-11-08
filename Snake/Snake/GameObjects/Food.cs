namespace SnakeGame.GameObjects
{
    public sealed class Food
    {
        private readonly char symbol;

        public Food(Coordinates coordinates, TimeSpan lifeTime)
        {
            this.symbol = '@';
            this.StartTime = DateTime.UtcNow;
            this.LifeTime = lifeTime; 
            this.Coordinates = coordinates;
        }

        public Char Symbol => this.symbol;

        public DateTime StartTime { get; set; }

        public Coordinates Coordinates { get; set; }

        public TimeSpan LifeTime { get; set; }

        public bool IsExpired => DateTime.UtcNow - StartTime > LifeTime;
    }
}
