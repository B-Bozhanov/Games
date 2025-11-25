namespace SnakeGame.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    using SnakeGame.Common;
    using SnakeGame.Core.Controllers;
    using SnakeGame.Core.Controllers.Interfaces;
    using SnakeGame.Core.GameLoop;
    using SnakeGame.Core.GameLoop.Interfaces;
    using SnakeGame.Core.Scenes;
    using SnakeGame.Core.Scenes.Interfaces;
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.Input;
    using SnakeGame.Rendering;
    using SnakeGame.SnakeAI;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSnakeGame(this IServiceCollection services)
        {
            services.AddScoped<IGameTime, GameTime>();
            services.AddScoped<IRenderer, ConsoleRenderer>();
            services.AddScoped<IInputReader, ConsoleInputReader>();
            services.AddScoped<IObjectFactory, BaseObjectFactory>();
            services.AddScoped<ISnakeAiController, SnakeAiController>();
            services.AddScoped<IGameBoard, ConsoleGameBoard>();
            services.AddScoped<ITheme<char, ConsoleColor>, ConsoleTheme>();
            services.AddScoped<ISnakeAiController, SnakeAiController>();
            services.AddScoped<ISnakeController, SnakeController>();
            services.AddScoped<IGameEngine, GameEngine>();
            services.AddSingleton<IBoardConfig>(_ => new BoardConfig(
                GlobalConstants.GameConstants.PlayableBoardWidth,
                GlobalConstants.GameConstants.PlayableBoardHeight,
                GlobalConstants.GameConstants.HeaderHeight,
                GlobalConstants.GameConstants.WallsWidth));

            services.AddKeyedScoped<IGameScene, SceneManager>("gameEngine");
            services.AddKeyedScoped<IGameScene, GameplayScene>("gamePlay");
            services.AddKeyedScoped<IGameScene, MainMenuScene>("menu");
            services.AddKeyedScoped<IGameScene, PauseScene>("pause");
            services.AddKeyedScoped<ISnake, Snake>("snake");
            services.AddKeyedScoped<ISnake, SnakeEnеmy>("snakeEnimy");

            return services;
        }
    }
}
