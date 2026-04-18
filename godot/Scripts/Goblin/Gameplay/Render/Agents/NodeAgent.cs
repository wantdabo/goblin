using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Godot;
using System;

namespace Goblin.Gameplay.Render.Agents;

/// <summary>
/// 节点代理，对应 Unity 的 NodeAgent
/// </summary>
public class NodeAgent : Agent
{
    private static Node3D root;

    public static void SetRoot(Node3D r) => root = r;

    public Node3D node { get; private set; }

    protected override void OnReady()
    {
        node = ObjectPool.Get<Node3D>("NODE_GO_KEY");
        if (null == node)
        {
            node = new Node3D { Name = "Node", Visible = false };
        }
        root?.AddChild(node);

        WatchRIL<RIL_SPATIAL>(OnRILSpatial);
    }

    protected override void OnReset()
    {
        if (null == node) return;
        node.Visible = false;
        root?.RemoveChild(node);
        ObjectPool.Set(node, "NODE_GO_KEY");
        node = null;
    }

    private void OnRILSpatial(RIL_SPATIAL ril) => ChangeStatus(ChaseStatus.Chasing);

    protected override bool OnArrived()
    {
        if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return true;
        if (!node.Visible)
        {
            node.Position = ril.position.ToVector3();
            node.Rotation = ril.euler.ToVector3() * MathF.PI / 180f;
            node.Scale = Vector3.One * ril.scale.AsFloat();
            node.Visible = true;
            return true;
        }

        return node.Position == ril.position.ToVector3() &&
               node.Rotation == ril.euler.ToVector3() * MathF.PI / 180f &&
               node.Scale == Vector3.One * ril.scale.AsFloat();
    }

    protected override void OnFlash()
    {
        base.OnFlash();
        if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return;
        node.Position = ril.position.ToVector3();
        node.Rotation = ril.euler.ToVector3() * MathF.PI / 180f;
        node.Scale = Vector3.One * ril.scale.AsFloat();
    }
}