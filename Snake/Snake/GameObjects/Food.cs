namespace SnakeGame.GameObjects;

using SnakeGame.GameObjects.Abstractions.Base;
using SnakeGame.GameObjects.Enums;

public sealed class Food(Coordinates coordinates, TimeSpan lifeTime)
    : BaseItem(coordinates, lifeTime)

{
    public override Color Color { get; protected set; } = Color.Green;
}