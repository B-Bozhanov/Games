namespace SnakeGame.Core.Scenes
{
    using Microsoft.Extensions.DependencyInjection;

    using SnakeGame.Core.Scenes.Interfaces;

    public class SceneManager(
        [FromKeyedServices("gamePlay")] IGameScene gamePlayScene,
        [FromKeyedServices("menu")] IGameScene menu,
        [FromKeyedServices("pause")] IGameScene pause) : IGameScene
    {
        private readonly IGameScene gamePlayScene = gamePlayScene;
        private readonly IGameScene menu = menu;
        private readonly IGameScene pause = pause;

        public void Run()
        {
            this.menu.Run();
            this.gamePlayScene.Run();
        }
    }
}