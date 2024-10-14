using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    public struct RIL_SKILLPIPELINE_INFO : IRIL
    {
        public ushort id => RILDef.SKILLPIPELINE_INFO;
        public uint skillid { get; private set; }
        public byte state { get; private set; }
        public uint frame { get; private set; }
        public uint length { get; private set; }

        public RIL_SKILLPIPELINE_INFO(uint skillid, byte state, uint frame, uint length)
        {
            this.skillid = skillid;
            this.state = state;
            this.frame = frame;
            this.length = length;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_SKILLPIPELINE_INFO _other)
            {
                return _other.state == state && _other.frame == frame;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, SkillID -> {skillid}, State -> {state}, Frame -> {frame}, Length -> {length}";
        }
    }
}
