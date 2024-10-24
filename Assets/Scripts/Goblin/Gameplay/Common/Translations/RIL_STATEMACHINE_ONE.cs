using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 状态机一层渲染指令
    /// </summary>
    public struct RIL_STATEMACHINE_ONE : IRIL 
    {
        public ushort id => RILDef.STATEMACHINE_ONE;
        /// <summary>
        /// 状态
        /// </summary>
        public uint state { get; private set; }
        /// <summary>
        /// 上一个状态
        /// </summary>
        public uint laststate { get; private set; }
        /// <summary>
        /// 状态机层
        /// </summary>
        public byte layer { get; private set; }

        /// <summary>
        /// 状态机一层渲染指令
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="lastsate">上一个状态</param>
        /// <param name="layer">状态机层</param>
        public RIL_STATEMACHINE_ONE(uint state, uint lastsate, byte layer)
        {
            this.state = state;
            this.laststate = lastsate;
            this.layer = layer;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_STATEMACHINE_ZERO _other)
            {
                return state == _other.state && layer == _other.layer;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, State -> {state}, LastState -> {laststate}, Layer -> {layer}";
        }
    }
}
