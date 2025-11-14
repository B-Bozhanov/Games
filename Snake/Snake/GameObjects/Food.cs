namespace SnakeGame.GameObjects;

using SnakeGame.GameObjects.Abstractions.Base;

public sealed class Food(Coordinates coordinates, TimeSpan lifeTime) 
    : BaseItem(coordinates, lifeTime)
    
{
}
