namespace Goblin.Gameplay.Logic.Commands.Common
{
    /// <summary>
    /// 输入指令
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// 指令
        /// </summary>
        public abstract ushort id { get; }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="clone">目标</param>
        /// <returns>目标克隆后</returns>
        public Command Clone(Command clone)
        {
            OnClone(clone);

            return clone;
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        protected abstract void OnReset();
        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="clone">目标</param>
        protected abstract void OnClone(Command clone);
    }
}