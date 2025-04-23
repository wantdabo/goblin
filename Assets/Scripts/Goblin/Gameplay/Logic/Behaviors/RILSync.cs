using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// RIL/渲染指令同步
    /// </summary>
    public class RILSync : Behavior
    {
        /// <summary>
        /// 发送渲染指令
        /// </summary>
        /// <param name="type">RIL 类型</param>
        /// <param name="actor">ActorID</param>
        /// <param name="ril">渲染指令</param>
        public void Send<T>(byte type, ulong actor, T ril) where T : IRIL
        {
            var rilstate = ObjectCache.Get<RILState<T>>();
            rilstate.type = type;
            rilstate.actor = actor;
            rilstate.frame = stage.frame;
            rilstate.ril = ril;
            
            // 产生渲染状态
            stage.onril?.Invoke(rilstate);
        }
    }
}