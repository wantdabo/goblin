using GoblinFramework.Core;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.Command.Cmds
{
    public class SyncPosCmd : SyncCmd
    {
        public int dire;
        public int x, y, z;

        public override CType Type => CType.SyncPosCmd;
    }
}
