using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 状态机, 用于管理实体的状态切换
    /// </summary>
    public class StateMachine : Behavior<StateMachineInfo>
    {
        /// <summary>
        /// 尝试切换状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>YES/NO</returns>
        public bool TryChangeState(byte state)
        {
            if (info.current == state) return true;
            if (false == QueryPassState(state)) return false;

            ChangeState(state);

            return true;
        }
        
        /// <summary>
        /// 切换到指定状态
        /// </summary>
        /// <param name="state">状态</param>
        public void ChangeState(byte state)
        {
            info.last = info.current;
            info.current = state;
            
            if (false == stage.SeekBehavior(actor, out Facade facade)) return;
            if (STATE_DEFINE.CASTING == info.current)
            {
                facade.SetAnimation(STATE_DEFINE.CASTING);
                return;
            }
            
            facade.SetAnimation(info.current);
        }
        
        /// <summary>
        /// 查询当前状态是否可以切换到指定状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>YES/NO</returns>
        private bool QueryPassState(byte state)
        {
            if (STATE_DEFINE.PASSES.TryGetValue(info.current, out var passes) && passes.Contains(state))
            {
                return true;
            }

            return false;
        }
    }
}