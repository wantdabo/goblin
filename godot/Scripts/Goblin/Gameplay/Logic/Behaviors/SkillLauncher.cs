using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Commands.Input;
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
    /// 装载技能
    /// </summary>
    public void Load(uint skill, FP strength, FP cooldown, ushort key, List<uint> pipelines)
    {
        if (info.loadedskilldict.ContainsKey(skill)) throw new Exception($"skill : {skill} already loaded.");
        var skillinfo = new SkillInfo
        {
            skill = skill,
            strength = strength,
            cooldown = cooldown,
            key = key,
            pipelines = pipelines
        };
        info.loadedskills.Add(skill);
        info.loadedskilldict.Add(skill, skillinfo);
    }

    /// <summary>
    /// 卸载技能
    /// </summary>
    public void Unload(uint skill)
    {
        if (false == info.loadedskilldict.TryGetValue(skill, out var skillinfo)) return;

        skillinfo.pipelines.Clear();
        ObjectCache.Set(skillinfo.pipelines);

        info.loadedskills.Remove(skill);
        info.loadedskilldict.Remove(skill);
    }

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
        if (false == info.loadedskilldict.TryGetValue(skill, out var skillinfo)) return;
        if (false == stage.SeekBehavior(actor, out StateMachine statemachine) || false == statemachine.TryChangeState(STATE_DEFINE.CASTING)) return;
        if (false == stage.SeekBehaviorInfo(actor, out SpatialInfo spatial)) return;

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
            pipelines = skillinfo.pipelines,
        });
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