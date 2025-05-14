using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Flows.Executors.Common
{
    /// <summary>
    /// 指令执行器
    /// </summary>
    public abstract class Executor
    {
        /// <summary>
        /// 场景
        /// </summary>
        protected Stage stage { get; private set; }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="stage">场景</param>
        /// <returns>指令执行器</returns>
        public Executor Load(Stage stage)
        {
            this.stage = stage;

            return this;
        }
        
        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload()
        {
            this.stage = null;
        }

        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        public void Enter(InstructData data, FlowInfo flowinfo)
        {
            OnEnter(data, flowinfo);
        }

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        public void Execute(InstructData data, FlowInfo flowinfo)
        {
            OnExecute(data, flowinfo);
        }
        
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        public void Exit(InstructData data, FlowInfo flowinfo)
        {
            OnExit(data, flowinfo);
        }
        
        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnEnter(InstructData data, FlowInfo flowinfo);
        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExecute(InstructData data, FlowInfo flowinfo);
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExit(InstructData data, FlowInfo flowinfo);
    }

    /// <summary>
    /// 指令执行器
    /// </summary>
    /// <typeparam name="T">指令数据类型</typeparam>
    public abstract class Executor<T> : Executor where T : InstructData
    {
        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected override void OnEnter(InstructData data, FlowInfo flowinfo)
        {
            OnEnter(data as T, flowinfo);
        }

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected override void OnExecute(InstructData data, FlowInfo flowinfo)
        {
            OnExecute(data as T, flowinfo);
        }

        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected override void OnExit(InstructData data, FlowInfo flowinfo)
        {
            OnExit(data as T, flowinfo);
        }

        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnEnter(T data, FlowInfo flowinfo);
        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExecute(T data, FlowInfo flowinfo);
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExit(T data, FlowInfo flowinfo);
    }
}