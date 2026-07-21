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
/// 动画代理，支持 AnimationTree（多层）和 AnimationPlayer（回退）
/// </summary>
public class AnimationAgent : Agent
{
    private string cfgname { get; set; }
    private AnimationConfig animcfg { get; set; }
    private AnimationPlayer animplayer { get; set; }
    private AnimationTree animtree { get; set; }
    private ModelAgent modelagent { get; set; }
    private string preplayname { get; set; }
    private string curplayname { get; set; }
    private string playname { get; set; }
    private float tarduration { get; set; }
    private float mixduration { get; set; }

    /// <summary>
    /// 是否已检出 AnimationTree（-1=未检出, 0=无, 1=有）
    /// </summary>
    private int treedetected { get; set; }

    protected override void OnReady()
    {
        cfgname = null; animcfg = null; animplayer = null; animtree = null; modelagent = null;
        preplayname = null; curplayname = null; playname = null;
        tarduration = 0; mixduration = 0; treedetected = -1;
        WatchRIL<RIL_FACADE_ANIMATION>(OnRILStateMachine);
    }

    protected override void OnReset()
    {
        cfgname = null; animcfg = null; animplayer = null; animtree = null; modelagent = null;
        preplayname = null; curplayname = null; playname = null;
        tarduration = 0; mixduration = 0; treedetected = -1;
    }

    private void OnRILStateMachine(RIL_FACADE_ANIMATION ril)
    {
        EnsureModel();
        if (null == modelagent?.node) return;
        EnsureAnimCfg();
        if (null == animcfg) return;
        TryDetectTree();

        if (null != animtree)
            DriveTree(ril);
        else
            RILConv2AnimData(ril);
    }

    /// <summary>
    /// AnimationTree 多层驱动
    /// </summary>
    private void DriveTree(RIL_FACADE_ANIMATION ril)
    {
        if (null == animplayer) return;
        for (int i = 0; i < ril.layercount; i++)
        {
            var entry = ril.layeranims[i];
            var nodename = animcfg.GetLayerNodeName(entry.layer);
            if (null == nodename) continue;

            var animname = ResolveAnimNameByEntry(entry);
            if (null == animname) continue;
            if (false == animplayer.HasAnimation(animname)) continue;

            animtree.Set($"parameters/{nodename}/animation", animname);
            animtree.Set($"parameters/{nodename}/seek_request", entry.elapsed * Config.Int2Float);
        }
    }

    /// <summary>
    /// 按层条目解析动画名（哈希优先，回退状态）
    /// </summary>
    private string ResolveAnimNameByEntry(LayerAnimEntry entry)
    {
        if (0 != entry.animhash)
        {
            var name = animcfg?.GetAnimationNameByHash(entry.animhash);
            if (null != name) return name;
        }
        return animcfg?.GetAnimationName(entry.animstate);
    }

    private void EnsureAnimCfg()
    {
        if (false == world.rilbucket.SeekRIL(actor, out RIL_FACADE_MODEL facademodel) || 0 >= facademodel.model) return;
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
    }

    private void RILConv2AnimData(RIL_FACADE_ANIMATION ril)
    {
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
    /// 解析动画名称（哈希优先，回退状态）—— AnimationPlayer 回退路径用
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

    private void EnsureModel()
    {
        if (null == modelagent || modelagent.actor != actor)
        {
            modelagent = world.GetAgent<ModelAgent>(actor);
            animplayer = null;
            animtree = null;
            treedetected = -1;
            playname = null; curplayname = null; preplayname = null;
        }
        if (null == modelagent?.node) return;
        if (null == animplayer)
            animplayer = modelagent.node.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
    }

    /// <summary>
    /// 延迟检出 AnimationTree（模型切换后重新检测）
    /// </summary>
    private void TryDetectTree()
    {
        if (null != animtree) return;
        if (0 == treedetected) return;
        treedetected = 0;
        if (null == modelagent?.node) return;
        animtree = modelagent.node.FindChild("AnimationTree", true, false) as AnimationTree;
        if (null != animtree)
        {
            animtree.Active = true;
            treedetected = 1;
        }
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
        if (null == animplayer || string.IsNullOrEmpty(playname)) return;
        if (!animplayer.HasAnimation(playname)) return;

        if (animplayer.CurrentAnimation != playname)
            animplayer.Play(playname, mixduration > 0 ? mixduration * (1f / timescale) : -1);

        animplayer.SpeedScale = 0;
        var newTime = Mathf.Clamp(animplayer.CurrentAnimationPosition + tick * timescale, 0, tarduration);
        animplayer.Seek(newTime, true);
    }
}