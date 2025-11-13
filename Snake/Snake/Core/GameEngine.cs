namespace SnakeGame.Core
{
    using Microsoft.Extensions.DependencyInjection;

    public class GameEngine(
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
