using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Attributes.Surfaces
{
    /// <summary>
    /// 外观
    /// </summary>
    public class Surface : Behavior<Translator>
    {
        /// <summary>
        /// 模型 ID
        /// </summary>
        public uint model { get; set; }
    }
}
