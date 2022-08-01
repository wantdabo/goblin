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
    /// <summary>
    /// Sync-Add-Resolver，渲染指令添加渲染节点解析
    /// </summary>
    public class SyncAddResolver : SyncResolver<SyncAddCmd>
    {
        protected override List<SyncCmd.CType> RelyResolvers => null;

        public GameObject Node = null;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnResolve<T>(T cmd)
        {
            if (null == Node)
            {
                Node = new GameObject(cmd.actorId.ToString());
                SelfReady = true;
            }
        }
    }
}
