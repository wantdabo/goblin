using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Flows.Common
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
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        public void Enter(Instruct instruct, FlowInfo flowinfo)
        {
            OnEnter(instruct, flowinfo);
        }

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        public void Execute(Instruct instruct, FlowInfo flowinfo)
        {
            OnExecute(instruct, flowinfo);
        }
        
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        public void Exit(Instruct instruct, FlowInfo flowinfo)
        {
            OnExit(instruct, flowinfo);
        }
        
        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnEnter(Instruct instruct, FlowInfo flowinfo);
        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExecute(Instruct instruct, FlowInfo flowinfo);
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExit(Instruct instruct, FlowInfo flowinfo);
    }

    /// <summary>
    /// 指令执行器
    /// </summary>
    /// <typeparam name="T">指令类型</typeparam>
    public abstract class Executor<T> : Executor where T : Instruct
    {
        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected override void OnEnter(Instruct instruct, FlowInfo flowinfo)
        {
            OnEnter(instruct as T, flowinfo);
        }

        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected override void OnExecute(Instruct instruct, FlowInfo flowinfo)
        {
            OnExecute(instruct as T, flowinfo);
        }

        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected override void OnExit(Instruct instruct, FlowInfo flowinfo)
        {
            OnExit(instruct as T, flowinfo);
        }

        /// <summary>
        /// 指令进入
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnEnter(T instruct, FlowInfo flowinfo);
        /// <summary>
        /// 指令执行
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExecute(T instruct, FlowInfo flowinfo);
        /// <summary>
        /// 指令离开
        /// </summary>
        /// <param name="instruct">指令</param>
        /// <param name="flowinfo">管线信息</param>
        protected abstract void OnExit(T instruct, FlowInfo flowinfo);
    }
}