using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 技能释放器
/// </summary>
public class SkillLauncher : Behavior<SkillLauncherInfo>
{
    /// <summary>
    /// 打断技能（移除当前 Magic Actor）
    /// </summary>
    public void Break()
    {
        if (false == info.casting) return;
        info.casting = false;
        if (info.magicid != 0 && stage.cache.Valid(info.magicid))
        {
            stage.RmvActor(info.magicid);
        }
        info.magicid = 0;
    }

    /// <summary>
    /// 释放技能（生成 Magic Actor）
    /// </summary>
    public void Launch(uint skill)
    {
        if (info.casting) return;
        if (false == stage.cfg.location.SkillInfos.TryGetValue((int)skill, out var skillcfg)) return;
        if (false == stage.SeekBehavior(actor, out StateMachine statemachine) || false == statemachine.TryChangeState(STATE_DEFINE.CASTING)) return;
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

        pipelines.Clear();
        ObjectCache.Set(pipelines);
    }

    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);

        // 消费技能指令
        if (false == info.casting && stage.SeekBehavior(actor, out Gamepad gamepad))
        {
            foreach (var skillcmd in gamepad.skills)
            {
                Launch(skillcmd.skillid);
                break;
            }
        }

        if (false == info.casting) return;

        // Magic Actor 消亡则技能结束
        if (info.magicid == 0 || false == stage.cache.Valid(info.magicid))
        {
            info.skill = 0;
            info.magicid = 0;
            info.casting = false;
        }

        if (stage.SeekBehavior(actor, out StateMachine statemachine))
        {
            if (false == info.casting && STATE_DEFINE.CASTING == statemachine.info.current)
            {
                statemachine.ChangeState(STATE_DEFINE.NONE);
            }
        }
    }
}
