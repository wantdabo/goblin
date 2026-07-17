using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 魔法体运动指令数据
/// </summary>
[MessagePackObject(true)]
public class MagicMotionData : InstructData
{
    public override ushort id => INSTR_DEFINE.MAGIC_MOTION;

    /// <summary>
    /// 运动类型
    /// </summary>
    public ushort motion;
    /// <summary>
    /// 速度（整数，除以 int2fp 得到 FP）
    /// </summary>
    public int speed;
    /// <summary>
    /// 速度倍率
    /// </summary>
    public int speedrate;
}
