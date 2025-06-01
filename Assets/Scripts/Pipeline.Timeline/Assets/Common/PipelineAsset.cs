using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Modules.UnityMathematics.Editor;
using UnityEngine;
using UnityEngine.Playables;

namespace Pipeline.Timeline.Assets.Common
{
    /// <summary>
    /// PlayableAsset 基类
    /// </summary>
    /// <typeparam name="T">管线行为</typeparam>
    /// <typeparam name="E">指令数据类型</typeparam>
    public class PipelineAsset<T, E> : PlayableAsset, IPlayableAsset where T : PipelineBehavior<E>, new() where E : InstructData
    {
        /// <summary>
        /// 指令数据
        /// </summary>
        [InlineProperty, HideLabel]
        [PropertySpace(SpaceAfter = 10)]
        public E data;
        
        [LabelText("条件列表")]
        public List<PipelineCondition> conditions;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph);
            playable.GetBehaviour().data = data;

            return playable;
        }
    }
}