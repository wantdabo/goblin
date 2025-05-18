using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 技能触发器
    /// </summary>
    public class SkillTrigger : Behavior
    {
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            // 获取到所有的技能释放器
            if (false == stage.SeekBehaviors(out List<SkillLauncher> launchers)) return;
            
            // 遍历所有的技能释放器, 检查 Gamepad 的输入, 是否响应技能释放
            foreach (var launcher in launchers)
            {
                if (false == stage.SeekBehavior(launcher.id, out Gamepad gamepad)) continue;
                foreach (var skill in launcher.info.loadedskills)
                {
                    var trigger = stage.cfg.location.SkillTriggerInfos.Get((int)skill);
                    if (null == trigger) continue;
                    if (false == gamepad.GetInput((ushort)trigger.Key).release) continue;
                    
                    // 技能释放
                    launcher.Launch(skill);
                }
            }
            
            launchers.Clear();
            ObjectCache.Set(launchers);
        }
    }
}