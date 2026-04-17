using Goblin.Core;
using System.Threading.Tasks;
using Godot;

namespace Goblin.Common.GameRes
{
    public class GameRes : Comp
    {
        public Location location;

        protected override void OnCreate()
        {
            base.OnCreate();
            location = AddComp<Location>();
            location.Create();
        }

        public async Task<T> LoadAssetAsync<T>(string res) where T : GodotObject
        {
            return await Task.FromResult(ResourceLoader.Load<T>(res));
        }

        public T LoadAssetSync<T>(string res) where T : GodotObject
        {
            return ResourceLoader.Load<T>(res);
        }

        public async Task<byte[]> LoadRawFileAsync(string res)
        {
            return await Task.FromResult(LoadRawFileSync(res));
        }

        public byte[] LoadRawFileSync(string res)
        {
            using var file = FileAccess.Open(res, FileAccess.ModeFlags.Read);
            return file?.GetBuffer((long)file.GetLength()) ?? System.Array.Empty<byte>();
        }
    }
}
