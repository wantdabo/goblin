using Goblin.Core;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace Goblin.Common.GameRes
{
    /// <summary>
    /// GameRes 资源加载组件
    /// </summary>
    public class GameRes : Comp
    {
        /// <summary>
        /// 资源加载定位器，具体的加载在这里实现
        /// </summary>
        public Location location;

        protected override void OnCreate()
        {
            base.OnCreate();
            location = AddComp<Location>();
            location.Create();
        }

        public async Task<T> LoadAssetAsync<T>(string res) where T : UnityEngine.Object
        {
            var handle = YooAssets.LoadAssetAsync<T>(res);
            await handle.Task;
            var result = handle.AssetObject as T;
            handle.Release();

            return result;
        }

        public T LoadAssetSync<T>(string res) where T : UnityEngine.Object
        {
            var handle = YooAssets.LoadAssetSync<T>(res);
            var result = handle.AssetObject as T;
            handle.Release();

            return result;
        }

        public async Task<byte[]> LoadRawFileAsync(string res)
        {
            var handle = YooAssets.LoadAssetSync<TextAsset>(res);
            await handle.Task;
            var ta = handle.AssetObject as TextAsset;
            handle.Release();

            return ta.bytes;
        }

        public byte[] LoadRawFileSync(string res) 
        {
            var handle  = YooAssets.LoadAssetSync<TextAsset>(res);
            var ta = handle.AssetObject as TextAsset;
            handle.Release();

            return ta.bytes;
        }

        public async Task<Scene> LoadSceneASync(string res, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var handle = YooAssets.LoadSceneAsync(res, loadSceneMode);
            await handle.Task;

            return handle.SceneObject;
        }
    }
}
