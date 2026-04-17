using Goblin.Common.GameRes;
using Goblin.Core;
using Godot;

namespace Goblin.Common.Sounds
{
    public class Sound : Comp
    {
        public void Unload(SoundInfo sound)
        {
            if (null == sound) return;
            sound.Stop();
            ObjectPool.Set(sound, $"SOUND_KEY{sound.res}");
        }

        public SoundInfo Load(string res)
        {
            var sound = ObjectPool.Get<SoundInfo>($"SOUND_KEY{res}");
            if (null == sound)
            {
                sound = AddComp<SoundInfo>();
                var stream = engine.gameres.LoadAssetSync<AudioStream>(Location.soundpath + res);
                sound.Initialize(res, stream);
                sound.Create();
            }

            if (sound.loop) return sound;

            engine.ticker.Timing((t) =>
            {
                sound.Stop();
                Unload(sound);
            }, sound.length, 1);

            return sound;
        }
    }
}
