using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Goblin.Sys.Common;

namespace Goblin.Sys.Gameplay
{
    public class GameplayProxy : Proxy<GameplayModel>
    {
        public GameplayDirector director { get; private set; }
        public bool physdraw { get; set; } = false;
        public bool showinfo { get; set; } = false;
        public bool dancing { get; set; } = false;
        public bool enemyautopilot { get; set; } = false;

        public void Load<T>(BuildData data, bool multithread = false) where T : GameplayDirector, new()
        {
            if (null != director) { director.StopGame(); director.Destroy(); }
            director = AddComp<T>();
            director.Create();
            director.CreateGame(data, multithread);
        }

        public void UnLoad()
        {
            if (null == director) return;
            director.StopGame();
            director.Destroy();
            director = null;
        }
    }
}
