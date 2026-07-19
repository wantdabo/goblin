using System.Collections.Generic;
using Goblin.Common;
using Goblin.Common.GameRes;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;
using Godot;

namespace Goblin.Gameplay.Render.Agents;

public class ModelAgent : Agent
{
    private static Node3D modelroot { get; set; }
    private static Node3D modelpool { get; set; }

    public static void SetRoot(Node3D root) => modelroot = root;
    public static void SetPool(Node3D pool) => modelpool = pool;

    public int model { get; private set; }
    public string res { get; private set; }
    public Node3D node { get; private set; }
    private SpatialAgent spatialnode { get; set; }
    private bool loaddirty { get; set; }

    protected override void OnReady()
    {
        RecycleModel();
        model = 0; res = null; node = null; spatialnode = null; loaddirty = true;
        WatchRIL<RIL_FACADE_MODEL>(OnRILFacadeModel);
    }

    protected override void OnReset()
    {
        RecycleModel();
        model = 0; res = null; node = null; spatialnode = null; loaddirty = false;
    }

    private void OnRILFacadeModel(RIL_FACADE_MODEL ril) => loaddirty = true;

    private void RecycleModel()
    {
        if (null == node) return;
        node.GetParent()?.RemoveChild(node);
        modelpool?.AddChild(node);
        node.Visible = false;
        ObjectPool.Set(node, res);
        node = null;
    }

    protected override void OnChase(float tick, float timescale)
    {
        base.OnChase(tick, timescale);
        if (loaddirty) { Load(); loaddirty = false; }

        if (null == node) return;
        if (spatialnode == null || spatialnode.actor != actor) spatialnode = world.GetAgent<SpatialAgent>(actor);
        if (null == spatialnode) return;
        node.Position = spatialnode.position;
        node.Quaternion = spatialnode.rotation;
        node.Scale = Vector3.One * spatialnode.scale;
        node.Visible = true;
    }

    public void Load()
    {
        if (false == world.rilbucket.SeekRIL(actor, out RIL_FACADE_MODEL facademodel) || 0 >= facademodel.model)
        {
            RecycleModel(); return;
        }
        if (model == facademodel.model) return;

        RecycleModel();
        model = facademodel.model;
        if (false == world.engine.cfg.location.ModelInfos.TryGetValue(facademodel.model, out var modelinfo)) return;
        res = modelinfo.Res;

        node = ObjectPool.Get<Node3D>(res);
        if (null == node || !GodotObject.IsInstanceValid(node))
        {
            var scene = world.engine.gameres.LoadAssetSync<PackedScene>(Location.modelpath + res + ".tscn");
            node = scene?.Instantiate<Node3D>();
        }
        if (null == node) return;

        modelroot?.AddChild(node);
        node.Position = Vector3.Zero;
        node.Rotation = Vector3.Zero;
        node.Scale = Vector3.One;
        node.Visible = false;
    }
}
