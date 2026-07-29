using Goblin.Gameplay.Logic.Commands.Common;
using Goblin.Gameplay.Logic.Commands.Soliders;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 传令官 — 接收全局指令，分发给 Solider 执行
/// </summary>
public class Herald : Behavior
{
    /// <summary>
    /// 输入指令队列
    /// </summary>
    private GBLQueue<Command> cmdqueue { get; set; }
    /// <summary>
    /// 输入指令执行器列表
    /// </summary>
    private GBLDict<ushort, Solider> soliderdict { get; set; }

    protected override void OnAssemble()
    {
        base.OnAssemble();

        cmdqueue = ObjectCache.Ensure<GBLQueue<Command>>();
            
        // 注册输入指令执行器
        soliderdict = ObjectCache.Ensure<GBLDict<ushort, Solider>>();
        void Solider<T>(ushort id) where T : Solider, new()
        {
            var solider = ObjectCache.Ensure<T>();
            soliderdict.Add(id, solider.Load(stage));
        }
        Solider<GMSolider>(INPUT_DEFINE.GM);
        Solider<TimeScaleSolider>(INPUT_DEFINE.TIMESCALE);
    }

    protected override void OnDisassemble()
    {
        base.OnDisassemble();
        cmdqueue.Dispose();
            
        // 卸载输入指令执行器
        foreach (var solider in soliderdict.Values)
        {
            solider.Unload();
        }
        // Dispose() 内部 Reset+Set 所有 Solider，再 Set 自身还池
        soliderdict.Dispose();
    }
        
    public void SetCommand(Command command)
    {
        if (null == command) return;
        cmdqueue.Enqueue(command);
    }

    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        // 执行输入指令
        while (cmdqueue.TryDequeue(out var command))
        {
            if (soliderdict.TryGetValue(command.id, out var solider)) solider.Execute(command);
        }
    }
}