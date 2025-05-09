namespace Goblin.Gameplay.Logic.Prefabs.Common
{
    /// <summary>
    /// 预制信息状态
    /// </summary>
    public abstract class PrefabInfoState
    {
        /// <summary>
        /// 重置预制信息状态
        /// </summary>
        public void Reset()
        {
            OnReset();
        }

        /// <summary>
        /// 重置预制信息状态
        /// </summary>
        protected abstract void OnReset();
    }
    
    /// <summary>
    /// 预制信息状态
    /// </summary>
    /// <typeparam name="T">预制信息类型</typeparam>
    public class PrefabInfoState<T> : PrefabInfoState where T : IPrefabInfo
    {
        /// <summary>
        /// 预制信息
        /// </summary>
        public T info { get; set; }
        
        /// <summary>
        /// 重置预制信息状态
        /// </summary>
        protected override void OnReset()
        {
            info = default;
        }
    }
}