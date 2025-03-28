namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 行为信息
    /// </summary>
    public interface IBehaviorInfo
    {
        public void Ready();
        
        public void Reset();
    }
}