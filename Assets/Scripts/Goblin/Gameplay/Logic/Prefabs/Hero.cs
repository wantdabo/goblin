using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Goblin.Gameplay.Prefabs.Common;

namespace Goblin.Gameplay.Prefabs
{
    public struct HeroInfo : IPrefabInfo
    {
        public int hero { get; set; }
        public Spatial spatial { get; set; }
        public Attribute attribute { get; set; }
    }

    public class Hero : Prefab<HeroInfo>
    {
        public override byte type => ACTOR_DEFINE.HERO;

        public override void OnProcessing(Actor actor, HeroInfo info)
        {
            actor.AddBehavior<Ticker>();
            actor.AddBehavior<StateMachine>();
            actor.AddBehavior<Movement>();
            
            var attribute = actor.AddBehaviorInfo<AttributeInfo>();
            attribute.hp = info.attribute.hp;
            attribute.maxhp = info.attribute.maxhp;
            attribute.moveseed = info.attribute.moveseed;
            attribute.attack = info.attribute.attack;
            
            var spatial = actor.AddBehaviorInfo<SpatialInfo>();
            spatial.position = info.spatial.position;
            spatial.euler = info.spatial.euler;
            spatial.scale = info.spatial.scale;

            if (actor.SeekBehavior(out Tag tag))
            {
                tag.Set(TAG_DEFINE.ACTOR_TYPE, type);
            }
        }
    }
}