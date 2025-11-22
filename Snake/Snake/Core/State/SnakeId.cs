namespace SnakeGame.Core.State
{
    public readonly record struct SnakeId(int Value)
    {
        public override string ToString() => this.Value.ToString();
    }
}
