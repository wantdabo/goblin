using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class HotfixState : GameStageState
    {
        public override List<Type> PassStates => new List<Type> { typeof(GameInitializeState) };
    }
}
