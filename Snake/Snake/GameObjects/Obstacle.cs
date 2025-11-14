namespace SnakeGame.GameObjects
{
    using System;

    using SnakeGame.GameObjects.Abstractions.Base;

    public class Obstacle(Coordinates coordinates, TimeSpan lifeTime) 
        : BaseItem(coordinates, lifeTime)
    {
    }
}
