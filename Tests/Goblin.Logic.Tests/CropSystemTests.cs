using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Projection.Rules;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.7 Crop 规则链 + Observer 测试
/// </summary>
[Collection("GBL")]
public class CropSystemTests
{
    // ============================================================
    // GodRule
    // ============================================================

    [Fact]
    public void GodRule_PassesAllFields()
    {
        var rule = new GodRule();
        var packet = new ProjectorPacket { fieldmask = 0b111 };
        var observer = new Observer { type = ObserverType.Player, id = 1 };

        var result = rule.Filter(packet, observer, 0b111);

        Assert.Equal(0b111ul, result);
    }

    [Fact]
    public void GodRule_PassesZeroMask()
    {
        var rule = new GodRule();
        var packet = new ProjectorPacket { fieldmask = 0 };
        var observer = new Observer { type = ObserverType.GM, id = 0 };

        var result = rule.Filter(packet, observer, 0);

        Assert.Equal(0ul, result);
    }

    // ============================================================
    // Crop 规则链
    // ============================================================

    [Fact]
    public void Crop_SingleRule_ChainsCorrectly()
    {
        var crop = new Crop();
        crop.AddRule(new GodRule());

        var packet = new ProjectorPacket { fieldmask = 0b101 };
        var observer = new Observer { type = ObserverType.Player, id = 1 };

        var result = crop.Project(packet, observer);

        Assert.Equal(0b101ul, result);
    }

    /// <summary>
    /// 模拟裁剪规则：mask 掉 index 0 字段
    /// </summary>
    private class MaskIndex0Rule : IProjectionRule
    {
        public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
        {
            return currentmask & ~1ul;
        }
    }

    /// <summary>
    /// 模拟裁剪规则：mask 掉 index 1 字段
    /// </summary>
    private class MaskIndex1Rule : IProjectionRule
    {
        public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
        {
            return currentmask & ~2ul;
        }
    }

    [Fact]
    public void Crop_MultipleRules_ChainFilters()
    {
        var crop = new Crop();
        crop.AddRule(new MaskIndex0Rule());
        crop.AddRule(new MaskIndex1Rule());

        var packet = new ProjectorPacket { fieldmask = 0b11 };
        var observer = new Observer { type = ObserverType.Player, id = 1 };

        var result = crop.Project(packet, observer);

        Assert.Equal(0ul, result);
    }

    [Fact]
    public void Crop_MaskZero_DropsPacket()
    {
        var crop = new Crop();
        // 这条规则会把一切 mask 为 0
        crop.AddRule(new MaskAllRule());

        var packet = new ProjectorPacket { fieldmask = 0b111 };
        var observer = new Observer { type = ObserverType.Player, id = 1 };

        var result = crop.Project(packet, observer);

        Assert.Equal(0ul, result);
    }

    private class MaskAllRule : IProjectionRule
    {
        public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
        {
            return 0;
        }
    }

    // ============================================================
    // Crop.Process — 多 Observer 裁剪
    // ============================================================

    [Fact]
    public void CropProcess_MultipleObservers_ProducesPerObserverPackets()
    {
        var crop = new Crop();
        crop.AddRule(new GodRule());

        var observers = new List<Observer>();
        observers.Add(new Observer { type = ObserverType.Player, id = 1, crop = crop });
        observers.Add(new Observer { type = ObserverType.GM, id = 0, crop = crop });

        var packet = new ProjectorPacket
        {
            actor = 42,
            behaviorinfotype = typeof(TestProjectInfo),
            fieldmask = 0b11,
            frame = 5,
            values = new object[] { 100, 200 },
        };

        var results = Crop.Process(new[] { packet }, observers);

        Assert.Equal(2, results.Length);

        Assert.Equal(1ul, results[0].observer.id);
        Assert.Equal(ObserverType.Player, results[0].observer.type);
        Assert.Equal(42ul, results[0].actor);
        Assert.Equal(0b11ul, results[0].fieldmask);

        Assert.Equal(0ul, results[1].observer.id);
        Assert.Equal(ObserverType.GM, results[1].observer.type);
    }

    // ============================================================
    // Observer
    // ============================================================

    [Fact]
    public void Observer_DefaultValues()
    {
        var obs = new Observer();

        Assert.Equal(ObserverType.GM, obs.type);
        Assert.Equal(0ul, obs.id);
    }

    [Fact]
    public void Observer_SetValues()
    {
        var obs = new Observer { type = ObserverType.GM, id = 99 };

        Assert.Equal(ObserverType.GM, obs.type);
        Assert.Equal(99ul, obs.id);
    }

    [Fact]
    public void ObserverType_AllValuesDefined()
    {
        Assert.Equal(0, (int)ObserverType.GM);
        Assert.Equal(1, (int)ObserverType.Editor);
        Assert.Equal(2, (int)ObserverType.Replay);
        Assert.Equal(3, (int)ObserverType.Player);
        Assert.Equal(4, (int)ObserverType.Spectator);
        Assert.Equal(5, (int)ObserverType.AI);
    }

    // ============================================================
    // ObserverPacket
    // ============================================================

    [Fact]
    public void ObserverPacket_DefaultValues()
    {
        var packet = new ObserverPacket();

        Assert.Null(packet.observer);
        Assert.Equal(0ul, packet.actor);
        Assert.Null(packet.behaviorinfotype);
        Assert.Equal(0ul, packet.fieldmask);
        Assert.Equal(0L, packet.frame);
        Assert.Null(packet.values);
    }

    [Fact]
    public void ObserverPacket_SetProperties()
    {
        var obs = new Observer { type = ObserverType.Player, id = 7 };
        var values = new object[] { 1, 2, 3 };
        var packet = new ObserverPacket
        {
            observer = obs,
            actor = 10,
            behaviorinfotype = typeof(TestProjectInfo),
            fieldmask = 7,
            frame = 20,
            values = values,
        };

        Assert.Same(obs, packet.observer);
        Assert.Equal(10ul, packet.actor);
        Assert.Equal(typeof(TestProjectInfo), packet.behaviorinfotype);
        Assert.Equal(7ul, packet.fieldmask);
        Assert.Equal(20L, packet.frame);
        Assert.Same(values, packet.values);
    }
}
