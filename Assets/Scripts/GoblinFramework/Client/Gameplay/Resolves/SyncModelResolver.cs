using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Gameplay.Resolves
{
    public class SyncModelResolver : SyncResolver<SyncModelCmd>
    {
        public override void Resolve<T>(T cmd)
        {
            base.Resolve(cmd);
        }
    }
}