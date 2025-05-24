using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Commands.Common
{
    /// <summary>
    /// 输入指令执行器
    /// </summary>
    public abstract class Solider
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
        public Solider Load(Stage stage)
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
        /// 执行输入指令
        /// </summary>
        /// <param name="command">输入指令</param>
        public virtual void Execute(Command command)
        {
        }
    }
    
    /// <summary>
    /// 输入指令执行器
    /// </summary>
    /// <typeparam name="T">输入指令类型</typeparam>
    public abstract class Solider<T> : Solider where T : Command
    {
        /// <summary>
        /// 执行输入指令
        /// </summary>
        /// <param name="command">输入指令</param>
        public override void Execute(Command command)
        {
            base.Execute(command);
            OnExecute(command as T);
        }

        /// <summary>
        /// 执行输入指令
        /// </summary>
        /// <param name="command">输入指令</param>
        protected abstract void OnExecute(T command);
    }
}