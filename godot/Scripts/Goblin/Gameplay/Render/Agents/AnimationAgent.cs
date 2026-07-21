using Goblin.Common;
using Goblin.Common.GameRes;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Core;
using Godot;
using System.Text.Json;

namespace Goblin.Gameplay.Render.Agents;

/// <summary>
/// 动画代理，使用 Godot AnimationPlayer 替代 Animancer
/// </summary>
public class AnimationAgent : Agent
{
    private string cfgname { get; set; }
    private AnimationConfig animcfg { get; set; }
    private AnimationPlayer animplayer { get; set; }
    private ModelAgent modelagent { get; set; }
    private string preplayname { get; set; }
    private string curplayname { get; set; }
    private string playname { get; set; }
    private float tarduration { get; set; }
    private float mixduration { get; set; }

    protected override void OnReady()
    {
        cfgname = null; animcfg = null; animplayer = null; modelagent = null;
        preplayname = null; curplayname = null; playname = null;
        tarduration = 0; mixduration = 0;
        WatchRIL<RIL_FACADE_ANIMATION>(OnRILStateMachine);
    }

    protected override void OnReset()
    {
        cfgname = null; animcfg = null; animplayer = null; modelagent = null;
        preplayname = null; curplayname = null; playname = null;
        tarduration = 0; mixduration = 0;
    }

    private void OnRILStateMachine(RIL_FACADE_ANIMATION ril)
    {
        // Phase 2: ril.layeranims 承载多层动画数据
        // layer 0 = 全身基础动画（ril.animstate/animhash 已由 Translator 镜像）
        // layer 1 = 上半身覆盖，layer 2 = 下半身覆盖
        // Phase 2.5: 根据 layeranims[1..] 驱动 AnimationTree 骨骼遮罩混合
        RILConv2AnimData(ril);
    }

    private void RILConv2AnimData(RIL_FACADE_ANIMATION ril)
    {
        if (false == world.rilbucket.SeekRIL(ril.actor, out RIL_FACADE_MODEL facademodel) || 0 >= facademodel.model) return;
        if (false == world.engine.cfg.location.ModelInfos.TryGetValue(facademodel.model, out var modelinfo)) return;

        if (string.IsNullOrEmpty(cfgname) || !modelinfo.Animation.Equals(cfgname))
        {
            cfgname = modelinfo.Animation;
            var bytes = world.engine.gameres.LoadRawFileSync(Location.animcfgpath + cfgname + ".json");
            if (bytes != null && bytes.Length > 0)
            {
                animcfg = JsonSerializer.Deserialize<AnimationConfig>(bytes);
                animcfg?.BuildHashIndex();
            }
        }

        var animname = ResolveAnimName(ril);
        if (curplayname != animname) { preplayname = curplayname; curplayname = animname; }

        playname = curplayname;
        mixduration = 0;
        tarduration = ril.animelapsed * Config.Int2Float;

        var animinfo = animcfg?.GetAnimationMixInfo(animname);
        if (null == animinfo) return;
        mixduration = animinfo.mixduration;

        if (null == preplayname) return;
        var beforeInfo = animinfo.GetAnimationBeforeMixInfo(preplayname);
        if (null != beforeInfo && tarduration < beforeInfo.duration)
        {
            playname = beforeInfo.name;
            mixduration = beforeInfo.mixduration;
        }
    }

    /// <summary>
    /// 解析动画名称（哈希优先，回退状态）
    /// </summary>
    private string ResolveAnimName(RIL_FACADE_ANIMATION ril)
    {
        if (0 != ril.animhash)
        {
            var name = animcfg?.GetAnimationNameByHash(ril.animhash);
            if (null != name) return name;
        }

        return animcfg?.GetAnimationName(ril.animstate);
    }

    protected override void OnFlash()
    {
        base.OnFlash();
        if (null == animplayer || string.IsNullOrEmpty(playname)) return;
        if (animplayer.HasAnimation(playname)) animplayer.Seek(tarduration, true);
    }

    protected override void OnChase(float tick, float timescale)
    {
        base.OnChase(tick, timescale);
        if (null == animplayer)
        {
            if (modelagent == null || modelagent.actor != actor) modelagent = world.GetAgent<ModelAgent>(actor);
            if (null == modelagent?.node) return;
            animplayer = modelagent.node.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
        }
        if (null == animplayer || string.IsNullOrEmpty(playname)) return;
        if (!animplayer.HasAnimation(playname)) return;

        if (animplayer.CurrentAnimation != playname)
            animplayer.Play(playname, mixduration > 0 ? mixduration * (1f / timescale) : -1);

        animplayer.SpeedScale = 0;
        var newTime = Mathf.Clamp(animplayer.CurrentAnimationPosition + tick * timescale, 0, tarduration);
        animplayer.Seek(newTime, true);
    }
}