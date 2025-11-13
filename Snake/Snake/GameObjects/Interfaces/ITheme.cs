namespace SnakeGame.GameObjects.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface ITheme<TSymbol>
    {
        public TSymbol Map(CellType cellType);
    }
}
