using GoblinFramework.Core;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics;
using GoblinFramework.Gameplay.Theaters;
using GoblinFramework.General;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay
{
    /// <summary>
    /// Play-Game-Engine 战斗的引擎组件
    /// </summary>
    public class PGEngine : GameEngine<PGEngine>
    {
        public TickEngine TickEngine = null;
        public World World = null;
        public Theater Theater = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            TickEngine = AddComp<TickEngine>();
            World = AddComp<World>();
            Theater = AddComp<Theater>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TickEngine = null;
            World = null;
            Theater = null;
        }

        public void SetInput(int actorId, InputType inputType, Input input) 
        {
            Theater.SetInput(actorId, inputType, input);
        }

        private Dictionary<object, Action<RIL>> rilRecvDict = new Dictionary<object, Action<RIL>>();
        public void RegisterRILRecv(object obj, Action<RIL> rilAction)
        {
            if (rilRecvDict.ContainsKey(obj)) throw new Exception("ril recv action repeat register.");
            rilRecvDict.Add(obj, rilAction);

            Theater.AddActor<Actors.Hoshi.HoshiActor>();
        }

        public void UnRegisterRILRecv(object obj)
        {
            if (false == rilRecvDict.ContainsKey(obj)) throw new Exception("ril recv action not found. plz check.");
            rilRecvDict.Remove(obj);
        }

        public void SendRIL<T>(T ril) where T : RIL
        {
            foreach (var kv in rilRecvDict) kv.Value.Invoke(ril);
        }
    }
}
