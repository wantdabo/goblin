using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    /// <summary>
    /// 技能触发器
    /// </summary>
    public class SkillBinding : Behavior
    {
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            // 获取到所有的技能释放器
            if (false == stage.SeekBehaviors(out List<SkillLauncher> launchers)) return;
            
            // 遍历所有的技能释放器, 检查 Gamepad 的输入, 是否响应技能释放
            foreach (var launcher in launchers)
            {
                if (false == stage.SeekBehavior(launcher.actor, out Gamepad gamepad)) continue;
                foreach (var skill in launcher.info.loadedskills)
                {
                    if (false == stage.cfg.location.SkillBindingInfos.TryGetValue((int)skill, out var binding)) return;
                    if (null == binding) continue;
                    if (false == gamepad.GetInput((ushort)binding.Key).press) continue;
                    
                    // 技能释放
                    launcher.Launch(skill);
                }
            }
            
            launchers.Clear();
            ObjectCache.Set(launchers);
        }
    }
}