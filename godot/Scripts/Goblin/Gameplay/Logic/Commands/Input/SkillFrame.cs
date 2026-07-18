namespace Goblin.Gameplay.Logic.Commands.Input;

/// <summary>
/// 技能帧输入 — 携带技能 ID，SkillLauncher 直接 Launch
/// </summary>
public class SkillFrame : InputFrame
{
    public uint skillid { get; set; }

    protected override void OnReady()
    {
        skillid = 0;
    }

    protected override void OnReset()
    {
        skillid = 0;
    }
}
