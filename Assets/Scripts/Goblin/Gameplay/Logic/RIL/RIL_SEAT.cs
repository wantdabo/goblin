using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 座位渲染指令
    /// </summary>
    public class RIL_SEAT : IRIL
    {
        public override ushort id => RIL_DEFINE.SEAT;
        
        /// <summary>
        /// 座位字典, 键为座位 ID, 值为 ActorID
        /// </summary>
        public Dictionary<ulong, ulong> seatdict { get; set; }

        public override void OnReady()
        {
            seatdict = ObjectCache.Get<Dictionary<ulong, ulong>>();
        }

        public override void OnReset()
        {
            seatdict.Clear();
            ObjectCache.Set(seatdict);
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public override int GetHashCode()
        {
            if (null == seatdict) return 0;

            int hash = 17;
            foreach (var kv in seatdict)
            {
                hash = hash * 31 + kv.Key.GetHashCode();
                hash = hash * 31 + kv.Value.GetHashCode();
            }
            
            return hash;
        }

        public override string ToString()
        {
            if (null == seatdict) return "RIL_SEAT: seatdict=null";
            if (seatdict.Count == 0) return "RIL_SEAT: seatdict={}";

            string result = "RIL_SEAT: seatdict={";
            foreach (var kv in seatdict)
            {
                result += $" {kv.Key}={kv.Value},";
            }
            result += " }";
            return result;
        }
    }
}