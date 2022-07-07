using GoblinFramework.Client;
using GoblinFramework.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    internal class GameEngineEntity : Entity
    {
        public LocationEntity Location = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            Location = CreateEntity<LocationEntity>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        internal T CreateEntity<T>() where T : Goblin, new()
        {
            T t = new T();
            t.Create(this);

            return t;
        }
    }
}
