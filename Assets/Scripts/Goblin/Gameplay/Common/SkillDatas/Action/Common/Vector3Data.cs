using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action.Common
{
    [MessagePackObject(true)]
    public class Vector3Data
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        
        public Vector3Data(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
