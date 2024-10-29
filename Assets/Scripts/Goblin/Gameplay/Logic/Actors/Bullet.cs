using Goblin.Gameplay.Logic.Attributes.Surfaces;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Physics;
using Goblin.Gameplay.Logic.Skills.Bullets.Common;
using Goblin.Gameplay.Logic.Spatials;
using System;

namespace Goblin.Gameplay.Logic.Actors
{
    /// <summary>
    /// 子弹
    /// </summary>
    public class Bullet : Actor
    {
        private SkillBullet skillbullet { get; set; }

        public new void Create()
        {
            throw new Exception("please use Create<T>().");
        }

        public void Create<T>() where T : SkillBullet, new()
        {
            skillbullet = AddBehavior<T>();
            base.Create();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            AddBehavior<Spatial>().Create();
            AddBehavior<PhysAgent>().Create();
            skillbullet.Create();
        }
    }
}
