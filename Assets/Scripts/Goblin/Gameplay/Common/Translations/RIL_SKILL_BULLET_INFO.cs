using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 子弹渲染指令
    /// </summary>
    public class RIL_SKILL_BULLET_INFO : IRIL
    {
        public ushort id => RILDef.SKILL_BULLET_INFO;

        /// <summary>
        /// 子弹 ID
        /// </summary>
        public uint bulletid { get; private set; }
        /// <summary>
        /// 子弹状态
        /// </summary>
        public byte state { get; private set; }
        /// <summary>
        /// 拥有者/ActorID
        /// </summary>
        public uint owner { get; private set; }

        /// <summary>
        /// 子弹渲染指令
        /// </summary>
        /// <param name="bulletid">子弹 ID</param>
        /// <param name="state">子弹状态</param>
        /// <param name="owner">拥有者/ActorID</param>
        public RIL_SKILL_BULLET_INFO(uint bulletid, byte state, uint owner)
        {
            this.bulletid = bulletid;
            this.state = state;
            this.owner = owner;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_SKILL_BULLET_INFO _other)
            {
                return _other.state == state && _other.owner == owner;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, bulletid -> {bulletid}, {state}, {owner}";
        }
    }
}
