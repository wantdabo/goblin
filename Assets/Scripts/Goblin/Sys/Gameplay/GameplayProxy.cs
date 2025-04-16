using Goblin.Common;
using Goblin.Gameplay.Directors.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Kowtow;
using Kowtow.Math;
using UnityEngine;

namespace Goblin.Sys.Gameplay
{
    /// <summary>
    /// 战斗 Proxy
    /// </summary>
    public class GameplayProxy : Proxy<GameplayModel>
    {
        public Director director { get; private set; }

        public void Load<T>(GPData data) where T : Director, new()
        {
            Time.fixedDeltaTime = GAME_DEFINE.LOGIC_TICK.AsFloat();
            if (null != director)
            {
                director.StopGame();
                director.Destroy();
            }

            director = AddComp<T>();
            director.Create();
            
            director.CreateGame(data);
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
