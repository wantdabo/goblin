using System.Collections.Generic;
using Goblin.Common;
using Goblin.Common.GameRes;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Godot;
using System;

namespace Goblin.Gameplay.Render.Agents;

/// <summary>
/// 特效代理
/// </summary>
public class EffectAgent : Agent
{
    private static Node3D root { get; set; }
    public static void SetRoot(Node3D r) => root = r;

    private Dictionary<uint, (EffectInfo info, EffectController controller)> effects { get; set; }
    private SpatialAgent spatialnode { get; set; }

    protected override void OnReady()
    {
        effects = ObjectPool.Ensure<Dictionary<uint, (EffectInfo, EffectController)>>();
        spatialnode = null;
        WatchRIL<RIL_FACADE_EFFECT>(OnRILFacadeEffect);
    }

    protected override void OnReset()
    {
        var rmv = ObjectPool.Ensure<List<uint>>();
        foreach (var kv in effects) rmv.Add(kv.Key);
        foreach (var id in rmv) RecycleEffect(id);
        rmv.Clear(); ObjectPool.Set(rmv);
        effects.Clear(); ObjectPool.Set(effects);
        spatialnode = null;
    }

    private void CreateEffect(EffectInfo info)
    {
        if (effects.ContainsKey(info.id)) return;
        var effcfg = world.engine.cfg.location.EffectInfos.GetOrDefault(info.effect);
        if (null == effcfg) return;

        var controller = ObjectPool.Get<EffectController>(effcfg.Res);
        if (null == controller || !GodotObject.IsInstanceValid(controller.node))
        {
            var scene = world.engine.gameres.LoadAssetSync<PackedScene>(Location.effectpath + effcfg.Res + ".tscn");
            var effnode = scene?.Instantiate<Node3D>();
            if (null == effnode) return;
            var pnodes = effnode.FindChildren("*", "CPUParticles3D", true, false);
            var anodes = effnode.FindChildren("*", "AnimationPlayer", true, false);
            var ps = new CpuParticles3D[pnodes.Count];
            var aps = new AnimationPlayer[anodes.Count];
            for (int i = 0; i < pnodes.Count; i++) ps[i] = pnodes[i] as CpuParticles3D;
            for (int i = 0; i < anodes.Count; i++) aps[i] = anodes[i] as AnimationPlayer;
            controller = new EffectController { node = effnode, particles = ps, animplayers = aps };
            root?.AddChild(effnode);
        }
        controller.node.Visible = true;
        effects.Add(info.id, (info, controller));
    }

    private void RecycleEffect(uint id)
    {
        if (false == effects.TryGetValue(id, out var effect)) return;
        effects.Remove(id);
        var effcfg = world.engine.cfg.location.EffectInfos.GetOrDefault(effect.info.effect);
        if (null == effcfg) return;
        effect.controller.node.Visible = false;
        effect.controller.Reset();
        ObjectPool.Set(effect.controller, effcfg.Res);
    }

    private void OnRILFacadeEffect(RIL_FACADE_EFFECT ril)
    {
        var rmv = ObjectPool.Ensure<List<uint>>();
        foreach (var kv in effects) if (false == ril.effectdict.ContainsKey(kv.Key)) rmv.Add(kv.Key);
        foreach (var id in rmv) RecycleEffect(id);
        rmv.Clear(); ObjectPool.Set(rmv);

        foreach (var kv in ril.effectdict)
        {
            if (effects.TryGetValue(kv.Key, out var effect))
            {
                effects.Remove(kv.Key);
                effects.Add(kv.Key, (kv.Value, effect.controller));
                continue;
            }
            CreateEffect(kv.Value);
        }
    }

    protected override void OnChase(float tick, float timescale)
    {
        base.OnChase(tick, timescale);
        foreach (var kv in effects)
        {
            var info = kv.Value.info;
            var controller = kv.Value.controller;

            var followpos = info.position.ToVector3();
            var followeuler = info.euler.ToVector3();
            var followscale = info.scale.AsFloat();

            if (info.follow == EFFECT_DEFINE.FOLLOW_ACTOR)
            {
                if (spatialnode == null || spatialnode.actor != actor) spatialnode = world.GetAgent<SpatialAgent>(actor);
                if (spatialnode != null)
                {
                    var rot = spatialnode.rotation.Normalized();
                    followpos = spatialnode.position + new Basis(rot) * followpos;
                    followeuler += rot.GetEuler() * 180f / MathF.PI;
                    followscale *= spatialnode.scale;
                }
            }

            if (EFFECT_DEFINE.FOLLOW_NONE != info.followmask)
            {
                if (EFFECT_DEFINE.FOLLOW_POSITION == (info.followmask & EFFECT_DEFINE.FOLLOW_POSITION))
                    controller.node.Position = followpos;
                if (EFFECT_DEFINE.FOLLOW_ROTATION == (info.followmask & EFFECT_DEFINE.FOLLOW_ROTATION))
                    controller.node.Rotation = followeuler * MathF.PI / 180f;
                if (EFFECT_DEFINE.FOLLOW_SCALE == (info.followmask & EFFECT_DEFINE.FOLLOW_SCALE))
                    controller.node.Scale = Vector3.One * followscale;
            }

            controller.Simulate(Mathf.Clamp(controller.time + tick * timescale, 0, info.elapsed.AsFloat()));
        }
    }
}