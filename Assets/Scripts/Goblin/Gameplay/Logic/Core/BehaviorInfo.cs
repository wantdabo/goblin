namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 行为信息
    /// </summary>
    public abstract class BehaviorInfo
    {
        public void Ready()
        {
            OnReady();
        }
        
        public void Reset()
        {
            OnReset();
        }
        
        protected abstract void OnReady();
        
        protected abstract void OnReset();
    }
}