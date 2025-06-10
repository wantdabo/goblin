using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Assets.Common
{
    /// <summary>
    /// PlayableAsset 基类
    /// </summary>
    public abstract class PipelineAsset : PlayableAsset, IPlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

        /// <summary>
        /// 指令数据
        /// </summary>
        public InstructData instrdata
        {
            get { return GetInstructData(); }
            set { SetInstructData(value); }
        }

        [LabelText("条件列表")]
        public List<PipelineCondition> conditions;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// 获取指令数据
        /// </summary>
        /// <returns>指令数据</returns>
        protected abstract InstructData GetInstructData();
        /// <summary>
        /// 设置指令数据
        /// </summary>
        /// <param name="data">指令数据</param>
        protected abstract void SetInstructData(InstructData data);
    }

    /// <summary>
    /// PlayableAsset 基类
    /// </summary>
    /// <typeparam name="T">管线行为</typeparam>
    /// <typeparam name="E">指令数据类型</typeparam>
    public class PipelineAsset<T, E> : PipelineAsset where T : PipelineBehavior<E>, new() where E : InstructData
    {
        /// <summary>
        /// 指令数据
        /// </summary>
        [InlineProperty, HideLabel]
        [PropertySpace(SpaceAfter = 10)]
        public E data;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph);
            playable.GetBehaviour().data = data;
            
            return playable;
        }

        protected override InstructData GetInstructData()
        {
            return data;
        }

        protected override void SetInstructData(InstructData data)
        {
            if (data is not E instructData) throw new System.InvalidCastException("cannot cast InstructData to " + typeof(E).Name);
            this.data = instructData;
        }
    }
}