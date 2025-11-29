namespace SnakeGame.GameObjects.Abstractions.Interfaces
{
    using SnakeGame.GameObjects.Enums;

    public interface ITheme<TSymbol, TColor>
    {
        public TSymbol MapSymbol(CellType cellType);

        public TColor MapColor(CellType cellType);
    }
}