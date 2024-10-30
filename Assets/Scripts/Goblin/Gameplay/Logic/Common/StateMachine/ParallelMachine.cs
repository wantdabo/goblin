using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Common.StateMachine
{
    /// <summary>
    /// 状态改变事件
    /// </summary>
    public struct StateChangedEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public byte layer { get; set; }
    }

    /// <summary>
    /// 状态进入事件
    /// </summary>
    public struct StateEnterEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public byte layer { get; set; }
    }

    /// <summary>
    /// 状态离开事件
    /// </summary>
    public struct StateExitEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public byte layer { get; set; }
    }

    /// <summary>
    /// 并发状态机
    /// </summary>
    public class ParallelMachine : Behavior<Translator>
    {
        /// <summary>
        /// 有限状态机集合
        /// </summary>
        private Machine[] machines = new Machine[StateDef.MAX_LAYER];

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
        }

        /// <summary>
        /// 获取状态机
        /// </summary>
        /// <param name="layer">层级</param>
        /// <returns>状态机</returns>
        public Machine GetMachine(byte layer = StateDef.LAYER_ZERO)
        {
            return machines[layer];
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="layer">层级</param>
        /// <returns>状态</returns>
        public T GetState<T>(byte layer = StateDef.LAYER_ZERO) where T : State
        {
            var machine = machines[layer];
            if (null == machine) return default;

            return machine.GetState<T>();
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="id">状态 ID</param>
        /// <param name="layer">层级</param>
        /// <returns>状态</returns>
        public State GetState(uint id, byte layer = StateDef.LAYER_ZERO)
        {
            var machine = machines[layer];
            if (null == machine) return default;

            return machine.GetState(id);
        }

        /// <summary>
        /// 设置状态机状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="layer">层级</param>
        public void SetState<T>(byte layer = StateDef.LAYER_ZERO) where T : State, new()
        {
            var machine = machines[layer];

            if (null == machine)
            {
                machine = AddComp<Machine>();
                machine.paramachine = this;
                machine.layer = layer;
                machines[layer] = machine;
                machine.Create();
            }

            machine.SetState<T>();
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
