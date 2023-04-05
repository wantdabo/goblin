using GoblinFramework.Logic.Common;

namespace GoblinFramework.Logic.Gameplay
{
    public class Behavior : LComp
    {
        public Actor actor;
    }

    public class BehaviorInfo : LComp
    {
    }

    public class Behavior<T> : Behavior where T : BehaviorInfo, new()
    {
        public T info;

        protected override void OnCreate()
        {
            base.OnCreate();

            info = AddComp<T>();
            info.Create();
        }

        protected override void OnDestroy()
        {
            RmvComp(info);
            info.Destroy();
            
            base.OnDestroy();
        }
    }
}