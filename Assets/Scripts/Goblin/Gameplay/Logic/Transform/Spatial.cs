using Goblin.Gameplay.Logic.Core;
using TrueSync;

namespace Goblin.Gameplay.Logic.Transform
{
    public class Spatial : Behavior<Translator>
    {
        public TSVector position { get; set; }
        public TSQuaternion rotation { get; set; }
        public TSVector scale { get; set; }
        public TSVector eulerAngle
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = TSQuaternion.Euler(value.x, value.y, value.z);
            }
        }
    }
}
