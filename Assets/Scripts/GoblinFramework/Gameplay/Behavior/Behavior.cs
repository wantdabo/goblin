using GoblinFramework.Client.Common;
using GoblinFramework.Common;
using GoblinFramework.Gameplay.Common;

namespace GoblinFramework.Gameplay
{
    public class Behavior : PComp
    {
        public Actor actor;
    }

    public class BehaviorInfo : PComp
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