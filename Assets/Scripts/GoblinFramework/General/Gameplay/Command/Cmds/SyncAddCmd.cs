using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.Command.Cmds
{
    public class SyncAddCmd : SyncCmd
    {
        ///// <summary>
        ///// 演员类型，用于分类演员
        ///// </summary>
        //public int actorType;
        public override CType Type => CType.SyncAddCmd;
    }
}
