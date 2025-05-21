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

        protected override void OnReady()
        {
            seatdict = ObjectCache.Ensure<Dictionary<ulong, ulong>>();
        }

        protected override void OnReset()
        {
            seatdict.Clear();
            ObjectCache.Set(seatdict);
        }

        protected override void OnCopy(IRIL target)
        {
            if (target is not RIL_SEAT ril) return;
            foreach (var kv in seatdict) ril.seatdict.Add(kv.Key, kv.Value);
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
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