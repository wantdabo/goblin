using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Sirenix.OdinInspector;
using UnityEngine.Playables;

namespace Pipeline.Timeline.Assets.Common
{
    /// <summary>
    /// 管线行为基类
    /// </summary>
    /// <typeparam name="T">指令数据类型</typeparam>
    public class PipelineBehavior<T> : PlayableBehaviour where T : InstructData
    {
        /// <summary>
        /// 条件列表
        /// </summary>
        [LabelText("条件列表")]
        [TableList]
        public List<Condition> conditions;
        /// <summary>
        /// 指令数据
        /// </summary>
        public T data;
    }
}