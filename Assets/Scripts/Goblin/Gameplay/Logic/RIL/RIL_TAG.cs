using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 标签渲染指令
    /// </summary>
    public struct RIL_TAG : IRIL
    {
        public ushort id => RIL_DEFINE.TAG;
        
        /// <summary>
        /// Actor 类型
        /// </summary>
        public int actortype { get; set; }
        /// <summary>
        /// 是否拥有英雄 ID
        /// </summary>
        public bool hashero { get; set; }
        /// <summary>
        /// 英雄 ID
        /// </summary>
        public int hero { get; set; }

        /// <summary>
        /// 标签渲染指令
        /// </summary>
        /// <param name="actortype">Actor 类型</param>
        /// <param name="hashero">是否拥有英雄 ID</param>
        /// <param name="hero">英雄 ID</param>
        public RIL_TAG(int actortype, bool hashero, int hero)
        {
            this.actortype = actortype;
            this.hashero = hashero;
            this.hero = hero;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            RIL_TAG ril = (RIL_TAG)other;
            
            return ril.id == id && ril.actortype == actortype && ril.hashero == hashero && ril.hero == hero;
        }

        public override string ToString()
        {
            return $"RIL_TAG: actortype={actortype}, hashero={hashero}, hero={hero}";
        }
    }
}