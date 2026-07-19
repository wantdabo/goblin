using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.EVENT;
using Goblin.Gameplay.Render.Core;
using Godot;

namespace Goblin.Gameplay.Render.Agents;

/// <summary>
/// 音效代理，空间音效播放、跟随 timescale 调 pitch、位置同步，由 RIL_EVENT_SOUND 驱动
/// </summary>
public class SoundAgent : Agent
{
    private List<int> oneshotplayers { get; set; }
    private Dictionary<uint, int> loopplayers { get; set; }
    private Queue<RIL_EVENT_SOUND> pendingqueue { get; set; }

    protected override void OnReady()
    {
        oneshotplayers = ObjectPool.Ensure<List<int>>();
        loopplayers = ObjectPool.Ensure<Dictionary<uint, int>>();
        pendingqueue = ObjectPool.Ensure<Queue<RIL_EVENT_SOUND>>();
    }

    protected override void OnReset()
    {
        foreach (var handle in oneshotplayers)
            world.engine.sound.StopSFX(handle);
        oneshotplayers.Clear();
        ObjectPool.Set(oneshotplayers);

        foreach (var kv in loopplayers)
            world.engine.sound.StopSFX(kv.Value);
        loopplayers.Clear();
        ObjectPool.Set(loopplayers);

        while (pendingqueue.TryDequeue(out var ril)) RILCache.Set(ril);
        ObjectPool.Set(pendingqueue);
    }

    /// <summary>
    /// 播放音效，由 SoundSalute 调用
    /// </summary>
    public void Play(RIL_EVENT_SOUND e)
    {
        var clone = RILCache.Ensure<RIL_EVENT_SOUND>();
        e.Clone(clone);
        pendingqueue.Enqueue(clone);
        ChangeStatus(ChaseStatus.Chasing);
    }

    protected override void OnChase(float tick, float timescale)
    {
        base.OnChase(tick, timescale);

        while (pendingqueue.Count > 0)
        {
            var e = pendingqueue.Dequeue();
            ExecutePlay(e);
            RILCache.Set(e);
        }

        var pos = world.GetAgent<SpatialAgent>(actor)?.position ?? Vector3.Zero;
        var sound = world.engine.sound;

        for (int i = oneshotplayers.Count - 1; i >= 0; i--)
        {
            var handle = oneshotplayers[i];
            if (false == sound.IsSFXValid(handle))
            {
                sound.StopSFX(handle);
                oneshotplayers.RemoveAt(i);
                continue;
            }

            if (false == sound.IsSFXPlaying(handle))
            {
                sound.StopSFX(handle);
                oneshotplayers.RemoveAt(i);
                continue;
            }

            sound.SetSFXPitch(handle, timescale);
            sound.SetSFXPosition(handle, pos);
        }

        var deadloops = ObjectPool.Ensure<List<uint>>();
        foreach (var kv in loopplayers)
        {
            var handle = kv.Value;
            if (false == sound.IsSFXValid(handle))
            {
                sound.StopSFX(handle);
                deadloops.Add(kv.Key);
                continue;
            }
            sound.SetSFXPitch(handle, timescale);
            sound.SetSFXPosition(handle, pos);
        }
        foreach (var key in deadloops) loopplayers.Remove(key);
        deadloops.Clear();
        ObjectPool.Set(deadloops);

        if (pendingqueue.Count == 0 && oneshotplayers.Count == 0 && loopplayers.Count == 0)
            ChangeStatus(ChaseStatus.Arrived);
    }

    private void ExecutePlay(RIL_EVENT_SOUND e)
    {
        var mode = (SoundMode)e.mode;

        switch (mode)
        {
            case SoundMode.Stop:
                StopSound(e.soundid);
                return;

            case SoundMode.OneShot:
            case SoundMode.Loop:
                PlaySound(e.soundid, mode == SoundMode.Loop);
                return;
        }
    }

    private void PlaySound(uint soundid, bool loop)
    {
        var sound = world.engine.sound;
        var config = sound.GetConfig(soundid);
        if (null == config) return;

        if (loop && loopplayers.TryGetValue(soundid, out var oldHandle))
        {
            sound.StopSFX(oldHandle);
            loopplayers.Remove(soundid);
        }

        var pos = world.GetAgent<SpatialAgent>(actor)?.position ?? Vector3.Zero;
        var handle = sound.PlaySFX(config.res, pos);
        if (handle < 0) return;

        sound.SetSFXVolume(handle, config.defaultvolume);

        if (loop)
            loopplayers[soundid] = handle;
        else
            oneshotplayers.Add(handle);
    }

    private void StopSound(uint soundid)
    {
        if (loopplayers.TryGetValue(soundid, out var handle))
        {
            world.engine.sound.StopSFX(handle);
            loopplayers.Remove(soundid);
        }
    }
}
