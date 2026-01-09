using Goblin.Gameplay.Logic.BehaviorInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Gameplay.Logic.Behaviors.Sa.RILEventInterfaces
{
    /// <summary>
    ///  自动化测试事件接口
    /// </summary>
    [RIL_EVENT]
    public interface IAutoTestEvent
    {
        public void OnTestDamageEvent(ulong from, ulong to, bool crit, int damageValue);

        public void OnCreateEffect(ulong actor, EffectInfo info);
    }
}
