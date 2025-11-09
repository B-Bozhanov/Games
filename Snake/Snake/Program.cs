using SnakeGame.Core;
using SnakeGame.GameObjects;
using SnakeGame.GameObjects.Interfaces;
using SnakeGame.Input;
using SnakeGame.Rendering;

ISnake snake = new Snake(0, 6);
IGameTime gameTime = new GameTime(targetFps: 13);
IRenderer renderer = new ConsoleRenderer();
IInputReader inputReader = new ConsoleInputReader();
IFoodFactory factory = new FoodFactory();

IGameScene gameScene = new GameEngine(inputReader, gameTime, renderer, factory, snake);

gameScene.Run();

