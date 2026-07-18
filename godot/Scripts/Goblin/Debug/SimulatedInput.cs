namespace Goblin.Debug;

/// <summary>
/// 模拟输入——LLM Agent 可通过 HTTP POST /input 注入。
/// </summary>
public class SimulatedInput
{
    /// <summary>目标座位号（从 Seat 查找 Actor）</summary>
    public ulong seat { get; set; }
    /// <summary>输入类型：JOYSTICK/BA/BB/BC</summary>
    public string type { get; set; } = "";
    /// <summary>是否按下</summary>
    public bool pressed { get; set; } = true;
    /// <summary>摇杆方向（JOYSTICK 时有效），值域 [-1000, 1000]</summary>
    public int direx { get; set; }
    /// <summary>摇杆方向 Y</summary>
    public int direy { get; set; }
}
