using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 直冲怪 AI 系统: 遍历所有 ChargeAIInfo, 写入对应 MovementInfo
/// 追击方向 + 分离力混合, 避免敌人堆叠
/// </summary>
public class ChargeAI : Behavior
{
    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (false == stage.SeekBehaviorInfos<ChargeAIInfo>(out var ais)) return;

        // 收集所有 Hero 位置, 每帧只查一次
        var heroes = ObjectCache.Ensure<List<(ulong actor, FPVector3 position)>>();
        stage.cache.AutoRecycle(heroes);
        if (stage.SeekBehaviors<Tag>(out var tags))
        {
            foreach (var tag in tags)
            {
                if (false == tag.Get(TAG_DEFINE.ACTOR_TYPE, out var t)) continue;
                if (t != ACTOR_DEFINE.HERO) continue;
                if (false == stage.SeekBehaviorInfo(tag.actor, out SpatialInfo sp)) continue;
                heroes.Add((tag.actor, sp.position));
            }
        }

        // 收集所有敌人位置, 用于分离力计算
        var enemypos = ObjectCache.Ensure<List<(ulong actor, FPVector3 position)>>();
        stage.cache.AutoRecycle(enemypos);
        foreach (var ai in ais)
        {
            if (false == stage.SeekBehaviorInfo(ai.actor, out SpatialInfo sp)) continue;
            enemypos.Add((ai.actor, sp.position));
        }

        // 分离半径 1.2m
        FP sepRadius = 1200 * stage.cfg.int2fp;

        foreach (var ai in ais)
        {
            if (false == stage.SeekBehaviorInfo(ai.actor, out SpatialInfo selfsp)) continue;
            if (false == stage.SeekBehaviorInfo(ai.actor, out MovementInfo movement)) continue;

            var target = FindNearestHero(heroes, selfsp.position, out var targetpos);
            ai.target = target;
            if (0 == target) { movement.wantmove = false; continue; }

            var diff = targetpos - selfsp.position;
            diff.y = FP.Zero;
            if (diff.sqrMagnitude <= ai.attackrange * ai.attackrange)
            {
                movement.wantmove = false;
                continue;
            }

            diff.Normalize();

            // 叠加分离力, 推开过近的同伴
            var sep = CalcSeparation(ai.actor, selfsp.position, enemypos, sepRadius);
            if (sep.sqrMagnitude > FP.Zero)
            {
                diff = diff + sep;
                diff.y = FP.Zero;
                diff.Normalize();
            }

            movement.dire = diff;
            movement.wantmove = true;
        }
    }

    private ulong FindNearestHero(List<(ulong actor, FPVector3 position)> heroes, FPVector3 selfpos, out FPVector3 targetpos)
    {
        targetpos = FPVector3.zero;
        ulong best = 0;
        FP bestsqr = FP.MaxValue;
        foreach (var hero in heroes)
        {
            var diff = hero.position - selfpos;
            diff.y = FP.Zero;
            var sqr = diff.sqrMagnitude;
            if (sqr < bestsqr) { bestsqr = sqr; best = hero.actor; targetpos = hero.position; }
        }
        return best;
    }

    // 分离力: 对半径内所有同伴叠加推开向量, 强度随距离线性衰减
    private FPVector3 CalcSeparation(ulong self, FPVector3 selfpos, List<(ulong actor, FPVector3 position)> others, FP radius)
    {
        var result = FPVector3.zero;
        var radiusSqr = radius * radius;
        foreach (var other in others)
        {
            if (other.actor == self) continue;
            var away = selfpos - other.position;
            away.y = FP.Zero;
            var sqr = away.sqrMagnitude;
            if (sqr >= radiusSqr || sqr == FP.Zero) continue;
            away.Normalize();
            result = result + away * (FP.One - sqr / radiusSqr);
        }
        return result;
    }
}
