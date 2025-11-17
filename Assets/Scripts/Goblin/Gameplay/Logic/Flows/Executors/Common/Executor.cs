using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Flows.Executors.Common
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
            stage = null;
        }

        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        public void Enter((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target)
        {
            OnEnter(identity, data, flowinfo, target);
        }

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        public void Execute((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target)
        {
            OnExecute(identity, data, flowinfo, target);
        }
        
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        public void Exit((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target)
        {
            OnExit(identity, data, flowinfo, target);
        }
        
        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected abstract void OnEnter((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target);
        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected abstract void OnExecute((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target);
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected abstract void OnExit((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target);
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
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected override void OnEnter((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target)
        {
            OnEnter(identity, data as T, flowinfo, target);
        }

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected override void OnExecute((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target)
        {
            OnExecute(identity, data as T, flowinfo, target);
        }

        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected override void OnExit((uint pipelineid, uint index) identity, InstructData data, FlowInfo flowinfo, ulong target)
        {
            OnExit(identity, data as T, flowinfo, target);
        }

        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected virtual void OnEnter((uint pipelineid, uint index) identity, T data, FlowInfo flowinfo, ulong target)
        {
        }

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected virtual void OnExecute((uint pipelineid, uint index) identity, T data, FlowInfo flowinfo, ulong target)
        {
        }

        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <param name="target">执行目标</param>
        protected virtual void OnExit((uint pipelineid, uint index) identity, T data, FlowInfo flowinfo, ulong target)
        {
        }
    }
}