using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.Prefabs.Datas;

namespace Goblin.Gameplay.Logic.Prefabs
{
    /// <summary>
    /// 英雄预设信息结构体
    /// </summary>
    public struct HeroPrefabInfo : IPrefabInfo
    {
        /// <summary>
        /// 英雄 ID
        /// </summary>
        public int hero { get; set; }
        /// <summary>
        /// 模型 ID
        /// </summary>
        public int model { get; set; }
        /// <summary>
        /// 空间信息
        /// </summary>
        public SpatialData spatial { get; set; }
        /// <summary>
        /// 属性信息
        /// </summary>
        public AttributeData attribute { get; set; }
    }

    /// <summary>
    /// 英雄预制创建器
    /// </summary>
    public class HeroPrefab : Prefab<HeroPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.HERO;

        protected override void OnProcessing(Actor actor, HeroPrefabInfo info)
        {
            actor.AddBehavior<StateMachine>();
            actor.AddBehavior<Movement>();

            actor.AddBehaviorInfo<TickerInfo>();
            
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
                tag.Set(TAG_DEFINE.HERO_ID, info.hero);
                tag.Set(TAG_DEFINE.MODEL_ID, info.model);
            }
        }
    }
}