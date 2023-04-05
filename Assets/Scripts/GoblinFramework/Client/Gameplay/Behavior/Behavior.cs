using GoblinFramework.Client.Common;

namespace GoblinFramework.Client.Gameplay
{
    public class Behavior : CComp
    {
        public Actor actor;
    }

    public class BehaviorInfo : CComp
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