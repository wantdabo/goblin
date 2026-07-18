using Goblin.Common;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;
using Godot;

namespace Goblin.Gameplay.Render.Agents;

/// <summary>
/// 程序化基元模型代理, 无 .tscn/.glb 依赖
/// 按 ModelInfo.Mesh 生成 BoxMesh/CylinderMesh/SphereMesh, 按 Size 设尺寸, 按 Color 设 albedo
/// </summary>
public class PrimitiveMeshAgent : Agent
{
    private static Node3D meshroot { get; set; }

    public static void SetRoot(Node3D root) => meshroot = root;

    public int model { get; private set; }
    public Node3D node { get; private set; }
    public MeshInstance3D meshinstance { get; private set; }
    public StandardMaterial3D material { get; private set; }
    private SpatialAgent spatialnode { get; set; }
    private bool loaddirty { get; set; }

    protected override void OnReady()
    {
        RecycleMesh();
        model = 0; node = null; meshinstance = null; material = null; spatialnode = null; loaddirty = true;
        WatchRIL<RIL_FACADE_MODEL>(OnRILFacadeModel);
    }

    protected override void OnReset()
    {
        RecycleMesh();
        model = 0; node = null; meshinstance = null; material = null; spatialnode = null; loaddirty = false;
    }

    private void OnRILFacadeModel(RIL_FACADE_MODEL ril) => loaddirty = true;

    private void RecycleMesh()
    {
        if (null == node) return;
        node.GetParent()?.RemoveChild(node);
        node.QueueFree();
        node = null;
        meshinstance = null;
        material = null;
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
            RecycleMesh(); return;
        }
        if (model == facademodel.model) return;

        RecycleMesh();
        model = facademodel.model;
        var modelinfo = world.engine.cfg.location.ModelInfos.GetOrDefault(facademodel.model);
        if (null == modelinfo) return;

        var size = ReadSize(modelinfo.Size);
        var color = ReadColor(modelinfo.Color);
        material = new StandardMaterial3D { AlbedoColor = color };

        Mesh mesh = modelinfo.Mesh switch
        {
            "box" => new BoxMesh { Size = size },
            "cylinder" => new CylinderMesh { TopRadius = size.X * 0.5f, BottomRadius = size.X * 0.5f, Height = size.Y },
            "sphere" => new SphereMesh { Radius = size.X * 0.5f, Height = size.Y },
            _ => new BoxMesh { Size = size },
        };

        node = new Node3D { Name = $"Primitive_{model}" };
        meshinstance = new MeshInstance3D { Mesh = mesh, MaterialOverride = material };
        // 身体沿 Y 抬到脚底贴地
        meshinstance.Position = new Vector3(0, size.Y * 0.5f, 0);
        node.AddChild(meshinstance);

        meshroot?.AddChild(node);
        node.Position = Vector3.Zero;
        node.Rotation = Vector3.Zero;
        node.Scale = Vector3.One;
        node.Visible = false;
    }

    private static Vector3 ReadSize(System.Collections.Generic.List<int> arr)
    {
        if (null == arr || arr.Count < 3) return new Vector3(1, 1, 1);
        return new Vector3(arr[0] * Config.Int2Float, arr[1] * Config.Int2Float, arr[2] * Config.Int2Float);
    }

    private static Color ReadColor(System.Collections.Generic.List<int> arr)
    {
        if (null == arr || arr.Count < 4) return new Color(1, 1, 1, 1);
        return new Color(arr[0] / 255f, arr[1] / 255f, arr[2] / 255f, arr[3] / 255f);
    }
}
