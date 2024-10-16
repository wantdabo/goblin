using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Attributes.Surfaces
{
    public class Surface : Behavior<Translator>
    {
        // TODO 后续要改为配置读取
        public uint model { get; set; } = 10000;
    }
}
