using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Flows
{
    /// <summary>
    /// Flow 碰撞 - 攻击盒信息
    /// </summary>
    public class FlowCollisionHurtInfo : FlowCollisionInfo
    {
        /// <summary>
        /// 使用[自身]命中火花
        /// </summary>
        public bool usesparkself { get; set; }
        /// <summary>
        /// 命中[自身]火花触发范围
        /// </summary>
        public sbyte sparkselfinfluence { get; set; }
        /// <summary>
        /// 命中[自身]火花令牌
        /// </summary>
        public string sparkselftoken { get; set; }
        /// <summary>
        /// 使用[目标]命中火花
        /// </summary>
        public bool usesparktarget { get; set; }
        /// <summary>
        /// 命中[目标]火花触发范围
        /// </summary>
        public sbyte sparktargetfluence { get; set; }
        /// <summary>
        /// 命中[目标]火花令牌
        /// </summary>
        public string sparktargettoken { get; set; }
    }
}