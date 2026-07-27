using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// HUD 展示行为，每帧将属性值从 AttributeBucket 同步到 HUDInfo 投影
/// </summary>
public class HUD : Behavior<HUDInfo>
{
    /// <summary>
    /// 组装时同步初始值，避免首帧 HUD 为空
    /// </summary>
    protected override void OnAssemble()
    {
        base.OnAssemble();
    }

    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        info.hp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.HP);
        info.maxhp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MAXHP);
        info.movespeed = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MOVESPEED);
        info.attack = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.ATTACK);
    }
}
