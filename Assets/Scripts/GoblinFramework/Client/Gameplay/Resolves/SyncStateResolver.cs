using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Gameplay.Resolves
{
    /// <summary>
    /// Sync-State-Resolver，渲染指令动画状态解析
    /// </summary>
    public class SyncStateResolver : SyncResolver<SyncStateCmd>
    {
        protected override List<SyncCmd.CType> RelyResolvers => new List<SyncCmd.CType> { SyncCmd.CType.SyncModelCmd };

        protected override void OnCreate()
        {
            base.OnCreate();
            SelfReady = true;
        }

        protected override void OnResolve<T>(T cmd)
        {
        }
    }
}