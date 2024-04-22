using Goblin.Common.Res;
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
    /// YooAsset 资源加载组件
    /// </summary>
    public class YooGameRes : GameRes
    {
        public override Task Initial()
        {
            YooAssets.Initialize();
            var package = YooAssets.CreatePackage("Package");
            YooAssets.SetDefaultPackage(package);
#if UNITY_EDITOR || UNITY_EDITOR_OSX
            var initParameters = new EditorSimulateModeParameters();
            var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("Package");
            initParameters.SimulateManifestFilePath = simulateManifestFilePath;
#elif YOOASSETS_OFFLINE
            var initParameters = new OfflinePlayModeParameters();
#endif
            return package.InitializeAsync(initParameters).Task;
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
            var handle = YooAssets.LoadRawFileAsync(resName);
            await handle.Task;

            return handle.GetRawFileData();
        }

        public override byte[] LoadRawFileSync(string resName) 
        {
            return YooAssets.LoadRawFileSync(resName).GetRawFileData();
        }

        public override async Task<Scene> LoadSceneASync(string resName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var handle = YooAssets.LoadSceneAsync(resName, loadSceneMode);
            await handle.Task;

            return handle.SceneObject;
        }
    }
}
