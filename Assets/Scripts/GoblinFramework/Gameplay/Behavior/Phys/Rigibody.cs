namespace GoblinFramework.Gameplay.Phys
{
    public class RigibodyInfo : BehaviorInfo
    {
        public PhysAssis physAssis;
    }

    public class Rigibody : Behavior<RigibodyInfo>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            info.physAssis = actor.GetBehavior<PhysAssis>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            info.physAssis = null;
        }
    }
}