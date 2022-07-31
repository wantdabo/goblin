using GoblinFramework.Gameplay.Actors.Hoshi.Behavior;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi
{
    public class HoshiActor : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            AddBehavior<InputBehavior>();
            AddBehavior<MotionBehavior>();
            AddBehavior<HoshiBehavior>();

            var addCmd = new SyncAddCmd();
            addCmd.actorId = ActorBehavior.Info.actorId;
            Engine.ToSyncCMD(addCmd);

            var modelCmd = new SyncModelCmd();
            modelCmd.actorId = ActorBehavior.Info.actorId;
            modelCmd.modelName = "Hoshi/Hoshi";
            Engine.ToSyncCMD(modelCmd);
        }
    }
}
