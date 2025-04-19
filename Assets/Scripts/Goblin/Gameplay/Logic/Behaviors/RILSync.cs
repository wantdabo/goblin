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
        public void Send(byte type, ulong actor, IRIL ril)
        {
            // 产生渲染状态
            stage.onril?.Invoke(new()
            {
                type = type,
                frame = stage.frame,
                actor = actor,
                ril = ril
            });
        }
    }
}