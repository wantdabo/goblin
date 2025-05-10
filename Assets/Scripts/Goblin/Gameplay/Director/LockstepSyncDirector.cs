using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;
using Queen.Protocols;

namespace Goblin.Gameplay.Director
{
    /// <summary>
    /// 锁步/帧同步导演
    /// </summary>
    public class LockstepSyncDirector : GameplayDirector
    {
        public override bool rendering { get; }
        
        protected override void OnCreateGame()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDestroyGame()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStartGame()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPauseGame()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnResumeGame()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStopGame()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnSnapshot()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRestore()
        {
            throw new System.NotImplementedException();
        }
    }
}