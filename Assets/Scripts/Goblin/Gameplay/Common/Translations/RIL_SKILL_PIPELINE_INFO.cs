using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 技能管线信息渲染指令
    /// </summary>
    public struct RIL_SKILL_PIPELINE_INFO : IRIL
    {
        public ushort id => RILDef.SKILL_PIPELINE_INFO;
        /// <summary>
        /// 技能 ID
        /// </summary>
        public uint skillid { get; private set; }
        /// <summary>
        /// 技能状态
        /// </summary>
        public byte state { get; private set; }
        /// <summary>
        /// 当前帧号
        /// </summary>
        public uint frame { get; private set; }
        /// <summary>
        /// 最大帧号
        /// </summary>
        public uint length { get; private set; }
    
        /// <summary>
        /// 技能管线信息渲染指令
        /// </summary>
        /// <param name="skillid"></param>
        /// <param name="state"></param>
        /// <param name="frame"></param>
        /// <param name="length"></param>
        public RIL_SKILL_PIPELINE_INFO(uint skillid, byte state, uint frame, uint length)
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
            if (other is RIL_SKILL_PIPELINE_INFO _other)
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
