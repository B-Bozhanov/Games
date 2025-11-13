using Microsoft.Extensions.DependencyInjection;

using SnakeGame.Core;
using SnakeGame.Extensions;

var services = new ServiceCollection().AddSnakeGame();

using ServiceProvider serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

var engine = scope.ServiceProvider.GetRequiredKeyedService<IGameScene>("gameEngine");
engine.Run();