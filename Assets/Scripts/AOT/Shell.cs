using Goblin.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace AOT
{
    /// <summary>
    /// Shell/游戏入口
    /// </summary>
    public class Shell : MonoBehaviour
    {
        private Task GameResSettings()
        {
            YooAssets.Initialize();
            var package = YooAssets.CreatePackage("Package");
            YooAssets.SetDefaultPackage(package);
#if UNITY_EDITOR || UNITY_EDITOR_OSX
            var initParameters = new EditorSimulateModeParameters();
            var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "Package");
            initParameters.SimulateManifestFilePath = simulateManifestFilePath;
#elif UNITY_WEBGL
            var initParameters = new WebPlayModeParameters();
            initParameters.BuildinQueryServices = new GameQueryServices();
            initParameters.RemoteServices = new RemoteServices();
#else
            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinQueryServices = new GameQueryServices();
            initParameters.DecryptionServices = new FileStreamDecryption();
            initParameters.RemoteServices = new RemoteServices();
#endif

            return package.InitializeAsync(initParameters).Task;
        }

#if HYBRID_CLR
        private async Task LoadMetadata()
        {
            var ta = await LoadScriptTASync("AOT_DLL_LIST");
            string[] aotDllList = ta.text.Split('|');
            foreach (var aotDllName in aotDllList)
            {
                ta = await LoadScriptTASync(aotDllName);
                var code = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(ta.bytes, HybridCLR.HomologousImageMode.SuperSet);
            }
        }

        private MethodInfo tickFunc { get; set; }
        private MethodInfo fixedTickFunc { get; set; }

        private async Task ScriptSettings()
        {
            await LoadMetadata();
            var ta = await LoadScriptTASync("Goblin.dll");
            Assembly ass = Assembly.Load(ta.bytes);
            var export = ass.GetType("Goblin.Core.Export");
            export.GetMethod("Init").Invoke(null, null);
            tickFunc = export.GetMethod("Tick");
            fixedTickFunc = export.GetMethod("FixedTick");
        }

        private async Task<TextAsset> LoadScriptTASync(string resName)
        {
            var handle = YooAssets.LoadAssetAsync<TextAsset>("Assets/GameRes/Raws/Scripts/" + resName);
            await handle.Task;
            var ta = handle.AssetObject as TextAsset;
            handle.Release();

            return ta;
        }
#endif

        private async void Start()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = int.MaxValue;

            await GameResSettings();
#if HYBRID_CLR
        await ScriptSettings();
        return;
#endif
            Export.Init();
        }

        private void Update()
        {
#if HYBRID_CLR
            if (null == tickFunc) return;
            tickFunc.Invoke(null, new object[] { Time.deltaTime });
            return;
#endif

            Export.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
#if HYBRID_CLR
            if (null == fixedTickFunc) return;
            fixedTickFunc.Invoke(null, new object[] { Time.fixedDeltaTime });
            return;
#endif

            Export.FixedTick(Time.fixedDeltaTime);
        }
    }
}
