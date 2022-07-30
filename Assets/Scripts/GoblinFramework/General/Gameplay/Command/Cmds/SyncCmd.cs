using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.Command.Cmds
{
    public abstract class SyncCmd
    {
        /// <summary>
        /// CmdType, CMD 的类型
        /// </summary>
        public enum CType
        {
            SyncCmd,
            SyncRmvCmd,
            SyncAddCmd,
            SyncPosCmd,
            SyncStateCmd,
            SyncModelCmd,
        }

        public int actorId;
        public abstract CType Type { get; }
    }
}
