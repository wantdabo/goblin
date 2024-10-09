using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// Behavior/行为
    /// </summary>
    public abstract class Behavior : Comp
    {
        /// <summary>
        /// 挂载的 Actor/实体
        /// </summary>
        public Actor actor { get; set; }
    }
}
