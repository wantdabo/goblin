using GoblinFramework.Client;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common
{
    internal class LocationEntity : Entity
    {
        internal EngineTickEntity EngineTick = null;

        protected override void OnCreate()
        {
            base.OnCreate();

            EngineTick = GameEngine.CreateEntity<EngineTickEntity>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
