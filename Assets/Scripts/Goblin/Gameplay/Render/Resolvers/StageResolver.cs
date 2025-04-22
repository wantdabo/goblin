using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StageResolver : Resolver<RIL_STAGE>
    {
        public override ushort id => RIL_DEFINE.STAGE;
        
        protected override IState OnRIL(RILState rilstate, RIL_STAGE ril)
        {
            return new StageState
            {
                actorcnt = ril.actorcnt,
                behaviorcnt = ril.behaviorcnt,
                behaviorinfocnt = ril.behaviorcnt,
                hassnapshot = ril.hassnapshot,
                snapshotframe = ril.snapshotframe,
            };
        }
    }
}