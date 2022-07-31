using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Gameplay.Resolves
{
    public class SyncModelResolver : SyncResolver<SyncModelCmd>
    {
        public GameObject Model;

        public override void Resolve<T>(T cmd)
        {
            base.Resolve(cmd);
            Model = Engine.GameRes.Location.LoadActorPrefabSync(cmd.modelName);
            Model.transform.SetParent(Actor.Node.transform);
        }
    }
}