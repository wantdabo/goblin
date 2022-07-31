using GoblinFramework.Client.Common;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Gameplay.Resolves
{
    public class SyncAddResolver : SyncResolver<SyncAddCmd>
    {
        public override void Resolve<T>(T cmd)
        {
            base.Resolve(cmd);
        }
    }
}
