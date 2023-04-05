using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

namespace GoblinFramework.Client.GameResource
{
    /// <summary>
    /// YooAsset 资源加载组件
    /// </summary>
    public class YooGameRes : GameRes
    {
        public override Task InitialGameRes()
        {
            base.OnCreate();
            
#if UNITY_EDITOR || UNITY_EDITOR_OSX
            var initParameters = new YooAssets.EditorSimulateModeParameters();
#elif YOOASSETS_OFFLINE
            var initParameters = new YooAssets.OfflinePlayModeParameters();
#else
            var initParameters = new YooAssets.HostPlayModeParameters();
            initParameters.DecryptionServices = null;
            initParameters.ClearCacheWhenDirty = false;
            initParameters.DefaultHostServer = "http://127.0.0.1/CDN1/Android";
            initParameters.FallbackHostServer = "http://127.0.0.1/CDN2/Android";
            initParameters.VerifyLevel = EVerifyLevel.High;
#endif
            initParameters.LocationServices = new DefaultLocationServices("Assets");
            var handle = YooAssets.InitializeAsync(initParameters);

            return handle.Task;
        }

        public async override Task<T> LoadAssetAsync<T>(string resName)
        {
            var handle = YooAssets.LoadAssetAsync<T>(resName);
            await handle.Task;

            return handle.AssetObject as T;
        }

        public override T LoadAssetSync<T>(string resName)
        {
            var handle = YooAssets.LoadAssetSync<T>(resName);

            return handle.AssetObject as T;
        }

        public override async Task<byte[]> LoadRawFileAsync(string resName)
        {
            var handle = YooAssets.GetRawFileAsync(resName);
            await handle.Task;

            return handle.LoadFileData();
        }

        public override async Task<Scene> LoadSceneASync(string resName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var handle = YooAssets.LoadSceneAsync(resName, loadSceneMode);
            await handle.Task;

            return handle.SceneObject;
        }
    }
}
