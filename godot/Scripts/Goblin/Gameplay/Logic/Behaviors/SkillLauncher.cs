using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 技能释放器（Sa 级）
/// 管理所有 Actor 的技能释放
/// </summary>
public class SkillLauncher : Behavior
{
    /// <summary>
    /// 打断技能
    /// </summary>
    public void Break(ulong actor)
    {
        if (false == stage.SeekBehaviorInfo(actor, out SkillLauncherInfo info)) return;
        if (false == info.casting) return;
        info.casting = false;
        if (info.magicid != 0 && stage.cache.Valid(info.magicid))
        {
            stage.RmvActor(info.magicid);
        }
        info.magicid = 0;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    public void Launch(ulong actor, uint skill)
    {
        if (false == stage.SeekBehaviorInfo(actor, out SkillLauncherInfo info)) return;
        if (info.casting) return;
        if (false == stage.cfg.location.SkillInfos.TryGetValue((int)skill, out var skillcfg)) return;
        if (false == stage.statemachine.TryChangeState(actor, STATE_DEFINE.CASTING)) return;
        if (false == stage.SeekBehaviorInfo(actor, out SpatialInfo spatial)) return;

        var pipelines = ObjectCache.Ensure<GBLList<uint>>();
        foreach (var p in skillcfg.Pipelines) pipelines.Add((uint)p);

        info.skill = skill;
        info.casting = true;
        info.magicid = stage.Spawn(new MagicPrefabInfo
        {
            owner = actor,
            spatial = new SpatialData
            {
                position = spatial.position,
                euler = spatial.euler,
                scale = spatial.scale,
            },
            pipelines = pipelines,
        });

        pipelines.Dispose();
    }

    protected override void OnTick(FP tick)
    {
        if (false == stage.SeekBehaviorInfos(out List<SkillLauncherInfo> infos)) return;
        foreach (var info in infos)
        {
            if (false == info.active) continue;
            var actor = info.actor;

            // 消费技能指令
            if (false == info.casting)
            {
                if (stage.SeekBehaviorInfo(actor, out GamepadInfo gamepadinfo)
                    && null != gamepadinfo.skills
                    && gamepadinfo.skills.Count > 0)
                {
                    Launch(actor, gamepadinfo.skills[0].skillid);
                }
            }

            if (false == info.casting) continue;

            // Magic Actor 消亡则技能结束
            if (info.magicid == 0 || false == stage.cache.Valid(info.magicid))
            {
                info.skill = 0;
                info.magicid = 0;
                info.casting = false;
            }

            if (false == info.casting
                && stage.SeekBehaviorInfo(actor, out StateMachineInfo sminfo)
                && STATE_DEFINE.CASTING == sminfo.current)
            {
                stage.statemachine.ChangeState(actor, STATE_DEFINE.NONE);
            }
        }
    }

    protected override void OnEndTick() { }
}
