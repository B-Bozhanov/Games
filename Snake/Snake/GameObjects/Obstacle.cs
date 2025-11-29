namespace SnakeGame.GameObjects
{
    using System;

    using SnakeGame.GameObjects.Abstractions.Base;
    using SnakeGame.GameObjects.Enums;

    public class Obstacle(Coordinates coordinates, TimeSpan lifeTime)
        : BaseItem(coordinates, lifeTime)
    {
        public override Color Color { get; protected set; } = Color.Cyan;
    }
}