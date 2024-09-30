using Goblin.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Common.FSM
{
    /// <summary>
    /// 状态改变事件
    /// </summary>
    public struct StateChangedEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state;
        /// <summary>
        /// 层级
        /// </summary>
        public int layer;
    }

    /// <summary>
    /// 状态进入事件
    /// </summary>
    public struct StateEnterEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state;
        /// <summary>
        /// 层级
        /// </summary>
        public int layer;
    }

    /// <summary>
    /// 状态离开事件
    /// </summary>
    public struct StateExitEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state;
        /// <summary>
        /// 层级
        /// </summary>
        public int layer;
    }

    /// <summary>
    /// 并发状态机
    /// </summary>
    public class ParallelMachine : Behavior
    {
        /// <summary>
        /// 最大层数
        /// </summary>
        public const byte MAX_LAYER = 2;
        /// <summary>
        /// 第零层
        /// </summary>
        public const byte LAYER_ZERO = 0;
        /// <summary>
        /// 第一层
        /// </summary>
        public const byte LAYER_ONE = 1;
        
        /// <summary>
        /// 有限状态机集合
        /// </summary>
        private Machine[] machines = new Machine[MAX_LAYER];
        /// <summary>
        /// 当前状态集合
        /// </summary>
        private State[] states = new State[MAX_LAYER];

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            actor.eventor.Listen<StateChangedEvent>(OnSMStateChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
            actor.eventor.UnListen<StateChangedEvent>(OnSMStateChanged);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="layer">层级</param>
        /// <returns>状态</returns>
        public T GetState<T>(int layer = LAYER_ZERO) where T : State
        {
            var machine = machines[layer];
            if (null == machine) return null;

            return machine.GetState<T>();
        }

        /// <summary>
        /// 设置状态机状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="layer">层级</param>
        public void SetState<T>(int layer = LAYER_ZERO) where T : State, new()
        {
            var machine = machines[layer];

            if (null == machine)
            {
                machine = AddComp<Machine>();
                machine.sm = this;
                machine.layer = layer;
                machines[layer] = machine;
                machine.Create();
            }

            machine.SetState<T>();
        }

        private void OnSMStateChanged(StateChangedEvent e)
        {
            states[e.layer] = e.state;
        }

        private void OnFPTick(FPTickEvent e)
        {
            for (int i = 0; i < machines.Length; i++)
            {
                var machine = machines[i];
                if (null != machine) machine.OnFPTick(e.frame, e.tick);
            }
        }
    }
}