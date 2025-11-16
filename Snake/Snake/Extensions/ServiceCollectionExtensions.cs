namespace SnakeGame.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    using SnakeGame.Core;
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Abstractions.Interfaces;
    using SnakeGame.Input;
    using SnakeGame.Rendering;
    using SnakeGame.Scenes;

    using SnakeGame.Common;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSnakeGame(this IServiceCollection services)
        {
            services.AddScoped<IGameTime, GameTime>();
            services.AddScoped<IRenderer, ConsoleRenderer>();
            services.AddScoped<IInputReader, ConsoleInputReader>();
            services.AddScoped<IObjectFactory, BaseObjectFactory>();
            services.AddScoped<IGameBoard, ConsoleGameBoard>();
            services.AddScoped<ITheme<char, ConsoleColor>, ConsoleTheme>();
            services.AddScoped<ISnakeAiController, BfsSnakeAiController>();
            services.AddSingleton<IBoardConfig>(_ => new BoardConfig(
                GlobalConstants.GameConstants.PlayableBoardWidth,
                GlobalConstants.GameConstants.PlayableBoardHeight,
                GlobalConstants.GameConstants.HeaderHeight,
                GlobalConstants.GameConstants.WallsWidth));

            services.AddKeyedScoped<IGameScene, GameEngine>("gameEngine");
            services.AddKeyedScoped<IGameScene, GameplayScene>("gamePlay");
            services.AddKeyedScoped<IGameScene, MainMenuScene>("menu");
            services.AddKeyedScoped<IGameScene, PauseScene>("pause");
            services.AddKeyedScoped<ISnake, Snake>("snake");
            services.AddKeyedScoped<ISnake, SnakeEnimy>("snakeEnimy");

            return services;
        }
    }
}
