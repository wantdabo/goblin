using System.Collections.Concurrent;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Commands.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Director.Common;

/// <summary>
/// 输入系统 — 管理指令队列和各输入槽状态
/// </summary>
public class InputSystem
{
    /// <summary>
    /// 指令队列
    /// </summary>
    private readonly ConcurrentQueue<Command> commandqueue = new();
    /// <summary>
    /// 输入槽，key → (press, dire)
    /// </summary>
    private readonly Dictionary<ushort, InputState> inputs = new();

    /// <summary>
    /// 入队指令
    /// </summary>
    public void EnqueueCommand(Command command)
    {
        commandqueue.Enqueue(command);
    }

    /// <summary>
    /// 出队指令
    /// </summary>
    public bool TryDequeueCommand(out Command command)
    {
        return commandqueue.TryDequeue(out command);
    }

    /// <summary>
    /// 获取输入槽状态
    /// </summary>
    public InputState GetInput(ushort key)
    {
        if (false == inputs.TryGetValue(key, out var state))
        {
            state = new InputState();
            inputs[key] = state;
        }
        return state;
    }
}

/// <summary>
/// 输入状态
/// </summary>
public struct InputState
{
    /// <summary>
    /// 是否按下
    /// </summary>
    public bool press;
    /// <summary>
    /// 方向
    /// </summary>
    public IntVector2 dire;
}
