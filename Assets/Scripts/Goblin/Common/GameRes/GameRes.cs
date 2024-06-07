using Goblin.Common.Res;
using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

namespace Goblin.Common.Res
{
    /// <summary>
    /// GameRes 资源加载组件
    /// </summary>
    public class GameRes : Comp
    {
        /// <summary>
        /// 资源加载定位器，具体的加载在这里实现
        /// </summary>
        public GameResLocation location;

        protected override void OnCreate()
        {
            base.OnCreate();
            location = AddComp<GameResLocation>();
            location.Create();
        }

        public async Task<T> LoadAssetAsync<T>(string resName) where T : UnityEngine.Object
        {
            var handle = YooAssets.LoadAssetAsync<T>(resName);
            await handle.Task;
            var result = handle.AssetObject as T;
            handle.Release();

            return result;
        }

        public T LoadAssetSync<T>(string resName) where T : UnityEngine.Object
        {
            var handle = YooAssets.LoadAssetSync<T>(resName);
            var result = handle.AssetObject as T;
            handle.Release();

            return result;
        }

        public async Task<byte[]> LoadRawFileAsync(string resName)
        {
            var handle = YooAssets.LoadAssetSync<TextAsset>(resName);
            await handle.Task;
            var ta = handle.AssetObject as TextAsset;
            handle.Release();

            return ta.bytes;
        }

        public byte[] LoadRawFileSync(string resName) 
        {
            var handle  = YooAssets.LoadAssetSync<TextAsset>(resName);
            var ta = handle.AssetObject as TextAsset;
            handle.Release();

            return ta.bytes;
        }

        public async Task<Scene> LoadSceneASync(string resName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var handle = YooAssets.LoadSceneAsync(resName, loadSceneMode);
            await handle.Task;

            return handle.SceneObject;
        }
    }
}
