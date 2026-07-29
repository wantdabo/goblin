using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// HUD 展示行为（Sa 级）
/// 每帧将属性值从 AttributeBucket 同步到 HUDInfo
/// </summary>
public class HUD : Behavior
{
    protected override void OnTick(FP tick)
    {
        if (false == stage.SeekBehaviorInfos(out List<HUDInfo> infos)) return;
        foreach (var info in infos)
        {
            if (false == info.active) continue;
            var actor = info.actor;
            info.hp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.HP);
            info.maxhp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MAXHP);
            info.movespeed = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MOVESPEED);
            info.attack = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.ATTACK);
        }
    }

    protected override void OnEndTick() { }
}
