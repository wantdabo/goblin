using Goblin.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// HUD 展示信息，从 AttributeBucket 转发到 Render 层
/// </summary>
// 当前生命值
[Projector("hp", typeof(int))]
// 最大生命值
[Projector("maxhp", typeof(int))]
// 移动速度
[Projector("movespeed", typeof(int))]
// 攻击力
[Projector("attack", typeof(int))]
public partial class HUDInfo : BehaviorInfo
{
}
