using Goblin.Core;
using Godot;

namespace Goblin.Common.Sounds;

/// <summary>
/// [已废弃] 逻辑已移入 Sound + SoundAgent
/// </summary>
public class SoundInfo : Comp
{
    public string res { get; private set; }
    public bool playing { get; private set; }
    public bool loop => player?.Stream is AudioStreamWav wav && wav.LoopMode != AudioStreamWav.LoopModeEnum.Disabled;
    public float length => (float)(player?.Stream?.GetLength() ?? 0.0);

    private AudioStreamPlayer3D player { get; set; }

    public void Initialize(string res, AudioStream stream)
    {
        this.res = res;
        player = new AudioStreamPlayer3D();
        player.Stream = stream;
    }

    public void Play()
    {
        playing = true;
        player?.Play();
    }

    public void Stop()
    {
        playing = false;
        player?.Stop();
    }
}