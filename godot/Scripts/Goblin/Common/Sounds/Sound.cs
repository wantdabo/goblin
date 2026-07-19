using System.Collections.Generic;
using Goblin.Common;
using Goblin.Common.GameRes;
using Goblin.Core;
using Godot;

namespace Goblin.Common.Sounds;

/// <summary>
/// 全局音效模块，负责 BGM 播放、UI 音效、音量/静音控制、soundid 查表
/// </summary>
public class Sound : Comp
{
    private int masterbusidx { get; set; }
    private int bgmbusidx { get; set; }
    private int sfxbusidx { get; set; }

    /// <summary>
    /// 音量/静音数据
    /// </summary>
    public SoundSettings settings { get; private set; }

    private AudioStreamPlayer bgmplayer { get; set; }
    private List<AudioStreamPlayer> uipool { get; set; }

    /// <summary>
    /// 3D 音效挂载根节点，由 World 注入
    /// </summary>
    private Node3D sfxroot { get; set; }

    /// <summary>
    /// 注入 3D 音效根节点
    /// </summary>
    public void SetSFXRoot(Node3D root) => sfxroot = root;

    private int sfxhandlecounter = 0;
    private Dictionary<int, AudioStreamPlayer3D> sfxplayers;

    /// <summary>
    /// soundid 到 SoundConfig 查表，与 SoundAgent 共用
    /// </summary>
    public Dictionary<uint, SoundConfig> soundconfigs { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        settings = new SoundSettings();
        sfxhandlecounter = 0;
        uipool = ObjectPool.Ensure<List<AudioStreamPlayer>>();
        sfxplayers = ObjectPool.Ensure<Dictionary<int, AudioStreamPlayer3D>>();
        soundconfigs = ObjectPool.Ensure<Dictionary<uint, SoundConfig>>();

        CacheBusIndices();
        ApplySettings();

        // 从配置表加载音效
        foreach (var info in engine.cfg.location.SoundInfos.DataList)
        {
            RegisterConfig(new SoundConfig
            {
                soundid = (uint)info.Id,
                res = info.Res,
                category = (SoundCategory)info.Category,
                defaultvolume = info.DefaultVolume
            });
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        bgmplayer?.Stop();
        bgmplayer?.QueueFree();
        bgmplayer = null;

        foreach (var player in uipool) player?.QueueFree();
        uipool.Clear();
        ObjectPool.Set(uipool);

        foreach (var kv in sfxplayers)
        {
            var player = kv.Value;
            if (null != player && GodotObject.IsInstanceValid(player))
            {
                player.Stop();
                player.Visible = false;
            }
        }
        sfxplayers.Clear();
        ObjectPool.Set(sfxplayers);

        soundconfigs.Clear();
        ObjectPool.Set(soundconfigs);
    }

    /// <summary>
    /// 确保 AudioBus 存在，不存在则自动创建
    /// </summary>
    private void CacheBusIndices()
    {
        masterbusidx = AudioServer.GetBusIndex("Master");

        bgmbusidx = AudioServer.GetBusIndex("BGM");
        if (bgmbusidx < 0)
        {
            bgmbusidx = AudioServer.GetBusCount();
            AudioServer.AddBus(bgmbusidx);
            AudioServer.SetBusName(bgmbusidx, "BGM");
            AudioServer.SetBusSend(bgmbusidx, "Master");
        }

        sfxbusidx = AudioServer.GetBusIndex("SFX");
        if (sfxbusidx < 0)
        {
            sfxbusidx = AudioServer.GetBusCount();
            AudioServer.AddBus(sfxbusidx);
            AudioServer.SetBusName(sfxbusidx, "SFX");
            AudioServer.SetBusSend(sfxbusidx, "Master");
        }
    }

    /// <summary>
    /// 播放 BGM
    /// </summary>
    public AudioStreamPlayer PlayBGM(uint soundid)
    {
        var config = GetConfig(soundid);
        if (null == config) return null;

        StopBGM();

        var stream = engine.gameres.LoadAssetSync<AudioStream>(Location.soundpath + config.res);
        if (null == stream) return null;

        bgmplayer = new AudioStreamPlayer { Stream = stream, Bus = "BGM" };
        bgmplayer.Finished += () =>
        {
            if (null != bgmplayer && GodotObject.IsInstanceValid(bgmplayer))
                bgmplayer.Play();
        };

        var sceneRoot = (Godot.Engine.GetMainLoop() as SceneTree)?.Root;
        sceneRoot?.AddChild(bgmplayer);
        bgmplayer.Play();
        return bgmplayer;
    }

    /// <summary>
    /// 停止 BGM
    /// </summary>
    public void StopBGM()
    {
        if (null == bgmplayer) return;
        if (GodotObject.IsInstanceValid(bgmplayer))
        {
            bgmplayer.Stop();
            bgmplayer.GetParent()?.RemoveChild(bgmplayer);
            bgmplayer.QueueFree();
        }
        bgmplayer = null;
    }

    /// <summary>
    /// 播放 UI 音效
    /// </summary>
    public AudioStreamPlayer PlayUI(uint soundid)
    {
        var config = GetConfig(soundid);
        if (null == config) return null;

        var stream = engine.gameres.LoadAssetSync<AudioStream>(Location.soundpath + config.res);
        if (null == stream) return null;

        var player = GetOrCreateUIPlayer();
        player.Stream = stream;
        player.Play();
        return player;
    }

    private AudioStreamPlayer GetOrCreateUIPlayer()
    {
        foreach (var p in uipool)
        {
            if (null == p || false == GodotObject.IsInstanceValid(p)) continue;
            if (false == p.Playing) return p;
        }

        var player = new AudioStreamPlayer { Bus = "SFX" };
        var sceneRoot = (Godot.Engine.GetMainLoop() as SceneTree)?.Root;
        sceneRoot?.AddChild(player);
        uipool.Add(player);
        return player;
    }

    /// <summary>
    /// 设置主音量 0.0 - 1.0
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        settings.mastervolume = Mathf.Clamp(volume, 0f, 1f);
        ApplyVolume(masterbusidx, settings.mastervolume);
    }

    /// <summary>
    /// 设置 BGM 音量 0.0 - 1.0
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        settings.bgmvolume = Mathf.Clamp(volume, 0f, 1f);
        ApplyVolume(bgmbusidx, settings.bgmvolume);
    }

    /// <summary>
    /// 设置 SFX 音量 0.0 - 1.0
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        settings.sfxvolume = Mathf.Clamp(volume, 0f, 1f);
        ApplyVolume(sfxbusidx, settings.sfxvolume);
    }

    private void ApplyVolume(int busidx, float volume)
    {
        if (busidx < 0) return;
        AudioServer.SetBusVolumeDb(busidx, Mathf.LinearToDb(volume));
    }

    /// <summary>
    /// 主静音
    /// </summary>
    public bool mastermuted
    {
        get => settings.mastermuted;
        set
        {
            settings.mastermuted = value;
            if (masterbusidx >= 0) AudioServer.SetBusMute(masterbusidx, value);
        }
    }

    /// <summary>
    /// BGM 静音
    /// </summary>
    public bool bgmmuted
    {
        get => settings.bgmmuted;
        set
        {
            settings.bgmmuted = value;
            if (bgmbusidx >= 0) AudioServer.SetBusMute(bgmbusidx, value);
        }
    }

    /// <summary>
    /// SFX 静音
    /// </summary>
    public bool sfxmuted
    {
        get => settings.sfxmuted;
        set
        {
            settings.sfxmuted = value;
            if (sfxbusidx >= 0) AudioServer.SetBusMute(sfxbusidx, value);
        }
    }

    /// <summary>
    /// 应用全部设置到 AudioServer
    /// </summary>
    private void ApplySettings()
    {
        ApplyVolume(masterbusidx, settings.mastervolume);
        ApplyVolume(bgmbusidx, settings.bgmvolume);
        ApplyVolume(sfxbusidx, settings.sfxvolume);
        if (masterbusidx >= 0) AudioServer.SetBusMute(masterbusidx, settings.mastermuted);
        if (bgmbusidx >= 0) AudioServer.SetBusMute(bgmbusidx, settings.bgmmuted);
        if (sfxbusidx >= 0) AudioServer.SetBusMute(sfxbusidx, settings.sfxmuted);
    }

    /// <summary>
    /// 注册音效配置
    /// </summary>
    public void RegisterConfig(SoundConfig config)
    {
        soundconfigs[config.soundid] = config;
    }

    /// <summary>
    /// 获取音效配置
    /// </summary>
    public SoundConfig GetConfig(uint soundid)
    {
        soundconfigs.TryGetValue(soundid, out var config);
        return config;
    }

    private AudioStreamPlayer3D AcquireSFXPlayer(string res)
    {
        var player = ObjectPool.Get<AudioStreamPlayer3D>(res);
        if (null == player || false == GodotObject.IsInstanceValid(player))
        {
            var stream = engine.gameres.LoadAssetSync<AudioStream>(Location.soundpath + res);
            if (null == stream) return null;
            player = new AudioStreamPlayer3D { Bus = "SFX", Stream = stream };
            player.SetMeta("SFX_PoolKey", res);
            sfxroot?.AddChild(player);
        }
        else
        {
            player.GetParent()?.RemoveChild(player);
            sfxroot?.AddChild(player);
        }
        player.Visible = true;
        return player;
    }

    private void ReleaseSFXPlayer(AudioStreamPlayer3D player)
    {
        if (null == player || false == GodotObject.IsInstanceValid(player)) return;
        player.Stop();
        player.Visible = false;

        if (false == player.HasMeta("SFX_PoolKey")) return;
        var res = player.GetMeta("SFX_PoolKey").AsString();
        ObjectPool.Set(player, res);
    }

    /// <summary>
    /// 播放 3D 音效，返回句柄（-1 表示失败）
    /// </summary>
    public int PlaySFX(string res, Vector3 position)
    {
        var player = AcquireSFXPlayer(res);
        if (null == player) return -1;
        player.GlobalPosition = position;
        player.Play();
        var handle = ++sfxhandlecounter;
        sfxplayers[handle] = player;
        return handle;
    }

    /// <summary>
    /// 停止并回收指定句柄的 SFX
    /// </summary>
    public void StopSFX(int handle)
    {
        if (sfxplayers.TryGetValue(handle, out var player))
        {
            ReleaseSFXPlayer(player);
            sfxplayers.Remove(handle);
        }
    }

    /// <summary>
    /// 设置 SFX 位置
    /// </summary>
    public void SetSFXPosition(int handle, Vector3 pos)
    {
        if (sfxplayers.TryGetValue(handle, out var player)
            && null != player && GodotObject.IsInstanceValid(player))
            player.GlobalPosition = pos;
    }

    /// <summary>
    /// 设置 SFX 音高
    /// </summary>
    public void SetSFXPitch(int handle, float pitch)
    {
        if (sfxplayers.TryGetValue(handle, out var player)
            && null != player && GodotObject.IsInstanceValid(player))
            player.PitchScale = pitch;
    }

    /// <summary>
    /// 设置 SFX 音量（0.0 - 1.0）
    /// </summary>
    public void SetSFXVolume(int handle, float volume)
    {
        if (sfxplayers.TryGetValue(handle, out var player)
            && null != player && GodotObject.IsInstanceValid(player))
            player.VolumeDb = Mathf.LinearToDb(volume);
    }

    /// <summary>
    /// 句柄对应 SFX 是否仍在播放
    /// </summary>
    public bool IsSFXPlaying(int handle)
    {
        return sfxplayers.TryGetValue(handle, out var player)
            && null != player && GodotObject.IsInstanceValid(player)
            && player.Playing;
    }

    /// <summary>
    /// 句柄是否有效（节点未销毁）
    /// </summary>
    public bool IsSFXValid(int handle)
    {
        return sfxplayers.TryGetValue(handle, out var player)
            && null != player && GodotObject.IsInstanceValid(player);
    }
}
