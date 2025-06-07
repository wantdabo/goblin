using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Assets.Common
{
    /// <summary>
    /// 指令预览基类
    /// </summary>
    public class PipelineBehavior : PlayableBehaviour
    {
    }
    
    /// <summary>
    /// 指令预览基类
    /// </summary>
    /// <typeparam name="T">指令数据类型</typeparam>
    public class PipelineBehavior<T> : PipelineBehavior where T : InstructData
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

        /// <summary>
        /// 是否进入过
        /// </summary>
        private bool passed = false;
        
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (passed) return;
            
            OnPass(playable, info);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            if (playable.GetTime() > playable.GetPreviousTime()) return;
            if (false == passed) return;
            
            OnReverse(playable, info);
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (false == passed) return;
            
            OnReverse(playable, default);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            OnExecute(playable, info);
        }

        protected virtual void OnPass(Playable playable, FrameData info)
        {
            passed = true;
        }
        
        protected virtual void OnReverse(Playable playable, FrameData info)
        {
            passed = false;
        }
        
        protected virtual void OnExecute(Playable playable, FrameData info)
        {
        }
    }
}