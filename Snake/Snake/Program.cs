using Microsoft.Extensions.DependencyInjection;

using SnakeGame.Core;
using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Interfaces;
using SnakeGame.Input;
using SnakeGame.Rendering;

IServiceCollection services = new ServiceCollection();
RegisterServices(services); 

using ServiceProvider serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

IGameScene gameScene = scope.ServiceProvider.GetRequiredService<IGameScene>();
gameScene.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddScoped<ISnake>(sp => new Snake(startPossitionRow: 0, length: 6));
    services.AddScoped<IGameTime>(sp => new GameTime(targetFps: 13));
    services.AddScoped<IRenderer, ConsoleRenderer>();
    services.AddScoped<IInputReader, ConsoleInputReader>();
    services.AddScoped<IFoodFactory, FoodFactory>();
    services.AddScoped<IGameScene, GameEngine>();
}