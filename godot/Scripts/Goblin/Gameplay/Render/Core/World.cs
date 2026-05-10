using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Batches;
using Goblin.Gameplay.Render.Cameras;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Godot;

namespace Goblin.Gameplay.Render.Core;

/// <summary>
/// 世界/渲染, 负责容纳所有的渲染层的单位
/// </summary>
public sealed class World : Comp
{
    /// <summary>
    /// Stage.ActorID, Stage 的数据走的也是 Actor/Behavior/BehaviorInfo 那一套
    /// 通过包装 Actor 的形式使用
    /// 所以 Stage 也是 Actor, 但它是一个特殊的 Actor, 它的 ID 是 ulong.MaxValue
    /// </summary>
    public ulong sa => ulong.MaxValue;
    /// <summary>
    /// 座位 ID
    /// </summary>
    private ulong mselfSeat = 0;
    public ulong selfseat => Interlocked.Read(ref mselfSeat);
    /// <summary>
    /// 自我
    /// </summary>
    public ulong self {
        get
        {
            if (false == rilbucket.SeekRIL<RIL_SEAT>(sa, out var ril)) return 0;
            if (false == ril.seatdict.TryGetValue(selfseat, out var actor)) return 0;
                
            return actor;
        }
    }
    /// <summary>
    /// 事件订阅派发者
    /// </summary>
    public Eventor eventor { get; private set; }
    /// <summary>
    /// Ticker/时间驱动器
    /// </summary>
    public Ticker ticker { get; private set; }
    /// <summary>
    /// 输入系统
    /// </summary>
    public InputSystem input { get; private set; }
    /// <summary>
    /// 桶
    /// </summary>
    public RILBucket rilbucket { get; private set; }
    /// <summary>
    /// 眼睛/摄像机
    /// </summary>
    public Eyes eyes { get; private set; }
    /// <summary>
    /// Agent 集合
    /// </summary>
    private Dictionary<ulong, Dictionary<Type, Agent>> agentdict { get; set; }
    /// <summary>
    /// 快照之后产生的 Agent
    /// </summary>
    private List<Agent> snapshotagents { get; set; }

    private Node3D worldroot { get; set; }
    private Node3D modelpool { get; set; }
    private DirectionalLight3D sunlight { get; set; }
    private WorldEnvironment worldenv { get; set; }
    private MeshInstance3D floormesh { get; set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        eventor = AddComp<Eventor>();
        eventor.Create();

        ticker = AddComp<Ticker>();
        ticker.Create();

        input = AddComp<InputSystem>();
        input.Initialize(this).Create();

        rilbucket = AddComp<RILBucket>();
        rilbucket.Initialize(this).Create();

        eyes = AddComp<Eyes>();
        eyes.Initialize(this).Create();

        var sceneRoot = (Godot.Engine.GetMainLoop() as SceneTree)?.Root;
        worldroot = new Node3D { Name = "WorldRoot" };
        modelpool = new Node3D { Name = "ModelPool", Visible = false };
        sceneRoot?.AddChild(worldroot);
        sceneRoot?.AddChild(modelpool);
        ModelAgent.SetRoot(worldroot);
        ModelAgent.SetPool(modelpool);
        PrimitiveMeshAgent.SetRoot(worldroot);
        EffectAgent.SetRoot(worldroot);

        SetupLighting();
        SetupFloor();

        Batches();

        agentdict = ObjectPool.Ensure<Dictionary<ulong, Dictionary<Type, Agent>>>();
        snapshotagents = ObjectPool.Ensure<List<Agent>>();

        ticker.eventor.Listen<TickEvent>(OnTick);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ticker.eventor.UnListen<TickEvent>(OnTick);
            
        // 回收所有 Agents
        foreach (var kv in agentdict)
        {
            foreach (var agent in kv.Value)
            {
                agent.Value.Reset();
                ObjectPool.Set(agent.Value);
            }
                
            kv.Value.Clear();
            ObjectPool.Set(kv.Value);
        }
        agentdict.Clear();
        ObjectPool.Set(agentdict);

        snapshotagents.Clear();
        ObjectPool.Set(snapshotagents);

        sunlight?.QueueFree(); sunlight = null;
        worldenv?.QueueFree(); worldenv = null;
        floormesh?.QueueFree(); floormesh = null;
        worldroot?.QueueFree(); worldroot = null;
        modelpool?.QueueFree(); modelpool = null;
        ModelAgent.SetRoot(null);
        ModelAgent.SetPool(null);
        PrimitiveMeshAgent.SetRoot(null);
        EffectAgent.SetRoot(null);
    }

    /// <summary>
    /// 程序化布置场景灯光与环境（无 .tscn 资源）
    /// </summary>
    private void SetupLighting()
    {
        sunlight = new DirectionalLight3D
        {
            Name = "SunLight",
            LightEnergy = 0.6f,
            LightColor = new Color(0.85f, 0.88f, 0.95f),
            ShadowEnabled = true,
            RotationDegrees = new Vector3(-50f, -45f, 0f),
        };
        worldroot.AddChild(sunlight);

        var env = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Color,
            BackgroundColor = new Color(0.08f, 0.10f, 0.14f),
            AmbientLightSource = Godot.Environment.AmbientSource.Color,
            AmbientLightColor = new Color(0.18f, 0.20f, 0.26f),
            AmbientLightEnergy = 0.4f,
        };
        worldenv = new WorldEnvironment { Name = "WorldEnv", Environment = env };
        worldroot.AddChild(worldenv);
    }

    /// <summary>
    /// 程序化棋盘格地面（无 .tscn 资源）
    /// 用 ShaderMaterial 内联 GLSL，每格 1m，深浅两色交替
    /// </summary>
    private void SetupFloor()
    {
        var shader = new Shader();
        shader.Code = @"
shader_type spatial;
render_mode unshaded;
uniform vec4 color_a : source_color = vec4(0.22, 0.22, 0.24, 1.0);
uniform vec4 color_b : source_color = vec4(0.15, 0.15, 0.17, 1.0);
uniform float cell_size = 1.0;
void fragment() {
    vec2 uv = UV * cell_size;
    float cx = floor(uv.x);
    float cy = floor(uv.y);
    float checker = mod(cx + cy, 2.0);
    ALBEDO = mix(color_a, color_b, checker).rgb;
}
";
        var mat = new ShaderMaterial { Shader = shader };
        mat.SetShaderParameter("cell_size", 64f);
        var mesh = new PlaneMesh { Size = new Vector2(64f, 64f), SubdivideDepth = 0, SubdivideWidth = 0 };
        // UV 铺满整个平面，每格 1m → UV 范围 = 64×64
        mesh.Material = mat;
        floormesh = new MeshInstance3D { Name = "Floor", Mesh = mesh };
        worldroot.AddChild(floormesh);
    }
        
    /// <summary>
    /// 初始化世界
    /// </summary>
    /// <param name="selfseat">我的座位号</param>
    /// <returns>世界</returns>
    public World Initialize(ulong selfseat)
    {
        Interlocked.Exchange(ref mselfSeat, selfseat);
            
        return this;
    }
        
    /// <summary>
    /// 创建批处理
    /// </summary>
    private void Batches()
    {
        AddComp<SpatialBatch>().Initialize(this).Create();
    }

    /// <summary>
    /// 拍摄
    /// </summary>
    public void Snapshot()
    {
        snapshotagents.Clear();
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public void Restore()
    {
        var agents = ObjectPool.Ensure<List<Agent>>();
        agents.AddRange(snapshotagents);
        foreach (var agent in agents) RmvAgent(agent);
        agents.Clear();
        ObjectPool.Set(agents);
        snapshotagents.Clear();
        foreach (var kv in agentdict)
        {
            foreach (var kv2 in kv.Value)
            {
                kv2.Value.Flash();
            }
        }
    }

    /// <summary>
    /// 切换座位
    /// </summary>
    /// <param name="seat">座位</param>
    public void SwitchSeat(ulong seat)
    {
        Interlocked.Exchange(ref mselfSeat, seat);
    }

    /// <summary>
    /// 获取 Agent, 如果不存在则创建
    /// </summary>
    /// <param name="actor">ActorID</param>
    /// <typeparam name="T">Agent 类型</typeparam>
    /// <returns>Agent</returns>
    public T EnsureAgent<T>(ulong actor) where T : Agent, new()
    {
        var agent = GetAgent<T>(actor);
        if (null == agent) agent = AddAgent<T>(actor);

        return agent;
    }

    /// <summary>
    /// 获取 Agent, 如果不存在则返回默认值
    /// </summary>
    /// <param name="actor">ActorID</param>
    /// <typeparam name="T">Agent 类型</typeparam>
    /// <returns>Agent</returns>
    public T GetAgent<T>(ulong actor) where T : Agent
    {
        if (false == agentdict.TryGetValue(actor, out var agents)) return default;
        if (false == agents.TryGetValue(typeof(T), out var agent)) return default;

        return agent as T;
    }
        
    /// <summary>
    /// 获取 Agent 集合
    /// </summary>
    /// <param name="actor">ActorID</param>
    /// <returns>Agent 集合</returns>
    public Dictionary<Type, Agent> GetAgents(ulong actor)
    {
        if (false == agentdict.TryGetValue(actor, out var agents)) return default;

        return agents;
    }

    /// <summary>
    /// 移除 Agents
    /// </summary>
    /// <param name="actor">ActorID</param>
    public void RmvAgent(ulong actor)
    {
        var dict = GetAgents(actor);
        if (null == dict) return;
        var agents = ObjectPool.Ensure<List<Agent>>();
        foreach (var kv in dict) agents.Add(kv.Value);
        foreach (var agent in agents) RmvAgent(agent);
        agents.Clear();
        ObjectPool.Set(agents);
    }

    /// <summary>
    /// 移除 Agent
    /// </summary>
    /// <param name="agent">Agent</param>
    public void RmvAgent(Agent agent)
    {
        var actor = agent.actor;
        if (false == agentdict.TryGetValue(actor, out var dict)) return;

        dict.Remove(agent.GetType());
        agent.Reset();
        ObjectPool.Set(agent);
        if (0 == dict.Count)
        {
            ObjectPool.Set(dict);
            agentdict.Remove(actor);
        }
            
        snapshotagents.Remove(agent);
    }

    /// <summary>
    /// 添加 Agent
    /// </summary>
    /// <param name="actor">ActorID</param>
    /// <typeparam name="T">Agent 类型</typeparam>
    /// <returns>Agent</returns>
    /// <exception cref="Exception">Agent 已存在</exception>
    private T AddAgent<T>(ulong actor) where T : Agent, new()
    {
        if (false == agentdict.TryGetValue(actor, out var dict))
        {
            agentdict.Add(actor, dict = ObjectPool.Ensure<Dictionary<Type, Agent>>());
        }

        if (dict.TryGetValue(typeof(T), out var agent)) throw new Exception($"agent {typeof(T)} already exists");
            
        agent = ObjectPool.Ensure<T>();
        agent.Ready(actor, this);
        dict.Add(typeof(T), agent);
        snapshotagents.Add(agent);

        return agent as T;
    }

    private void OnTick(TickEvent e)
    {
        // 执行过程中, 可能会触发修改 agentdict, 导致错误
        var agents = ObjectPool.Ensure<List<(Agent agents, float timescale)>>();
        foreach (var kv in agentdict)
        {
            float timescale = 1f;
            if (rilbucket.SeekRIL<RIL_TICKER>(kv.Key, out var ril)) timescale = ril.timescale * Config.Int2Float;
            foreach (var agent in kv.Value.Values)
            {
                if (ChaseStatus.Arrived == agent.status) continue;
                agents.Add((agent, timescale));
            }
        }

        // 收集后再处理
        foreach ((Agent agent, float timescale) in agents) agent.Chase(e.tick, timescale);
        agents.Clear();
        ObjectPool.Set(agents);
    }
}