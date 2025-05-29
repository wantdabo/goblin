using System.Threading.Tasks;
using Goblin.Core;
using UnityEngine;
using YooAsset;

/// <summary>
/// Entrance/游戏入口
/// </summary>
public class Entrance : MonoBehaviour
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

    private async void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = int.MaxValue;

        await GameResSettings();

        Export.Init();
    }

    private void Update()
    {
        Export.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Export.FixedTick(Time.fixedDeltaTime);
    }
}