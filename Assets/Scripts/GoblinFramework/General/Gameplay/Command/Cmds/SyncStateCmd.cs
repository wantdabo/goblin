using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.Command.Cmds
{
    public class SyncStateCmd : SyncCmd
    {
        public int stateId;

        public override CType Type => CType.SyncStateCmd;
    }
}
