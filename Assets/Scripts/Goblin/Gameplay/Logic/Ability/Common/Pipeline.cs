using System.Collections.Generic;
using Goblin.Gameplay.Logic.Ability.AIL.Common;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Logic.Ability.Common
{
    public sealed class Pipeline
    {
        private List<AILState> ailstates { get; set; }
        
        public void Insert<T>(T inst) where T : IAIL
        {
            var instruct = ObjectCache.Get<AILState<T>>();
            instruct.inst = inst;
        }
    }
}