namespace SnakeGame.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    using SnakeGame.Core;
    using SnakeGame.GameObjects;
    using SnakeGame.GameObjects.Interfaces;
    using SnakeGame.Input;
    using SnakeGame.Rendering;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSnakeGame(this IServiceCollection services)
        {
            services.AddScoped<ISnake, Snake>();
            services.AddScoped<IGameTime, GameTime>();
            services.AddScoped<IRenderer, ConsoleRenderer>();
            services.AddScoped<IInputReader, ConsoleInputReader>();
            services.AddScoped<IFoodFactory, FoodFactory>();
            services.AddScoped<IGameScene, GameEngine>();
            services.AddScoped<IGameBoard, ConsoleGameBoard>();
            services.AddScoped<ITheme<char>, ConsoleTheme>();

            return services;
        }
    }
}
