using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 外观渲染指令
    /// </summary>
    public class RIL_SURFACE : IRIL
    {
        public ushort id => RIL_DEFINE.SURFACE;
        
        /// <summary>
        /// 模型 ID
        /// </summary>
        public uint model { get; private set; }
        
        /// <summary>
        /// 外观渲染指令
        /// </summary>
        /// <param name="model">模型 ID</param>
        public RIL_SURFACE(uint model)
        {
            this.model = model;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_SURFACE _other)
            {
                return _other.model == model;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, model -> {model}";
        }
    }
}
