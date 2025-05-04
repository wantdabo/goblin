#if HYBRID_CLR
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace Goblin.Custom
{

    /// <summary>
    /// 打包编辑工具
    /// </summary>
    public class PackageEditor : Editor
    {
        [MenuItem("工具/构建/发布/发布 APK")]
        public static void BuildAPK()
        {
            BuildScripts();
            BuildAssetBundles(BuildTarget.Android);
            Release(BuildTarget.Android, "android/game.apk");
        }

        [MenuItem("工具/构建/发布/发布 EXE32")]
        public static void BuildEXE32()
        {
            BuildAssetBundles(BuildTarget.StandaloneWindows);
            Release(BuildTarget.StandaloneWindows, "win/32/game.exe");
        }

        [MenuItem("工具/构建/发布/发布 EXE64")]
        public static void BuildEXE64()
        {
            BuildAssetBundles(BuildTarget.StandaloneWindows64);
            Release(BuildTarget.StandaloneWindows64, "win/64/game.exe");
        }

        private static void Release(BuildTarget buildTarget, string fileName)
        {
            var scenes = EditorBuildSettings.scenes;
            string[] scenePaths = new string[scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenePaths[i] = scenes[i].path;
            }

            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(scenePaths, $"build/{fileName}", buildTarget, buildOptions);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + report.summary.totalSize + " bytes");
            }
            else
            {
                Debug.LogError("Build failed");
            }
        }

        private static BuildOptions buildOptions
        {
            get
            {
                return BuildOptions.Development | BuildOptions.ConnectWithProfiler;
            }
        }

        [MenuItem("工具/构建/补丁/APK")]
        public static void BuildPatchAPK()
        {
            BuildScripts();
            var version = BuildAssetBundles(BuildTarget.Android);
            var path = Application.dataPath.Replace("/Assets", "") + "/Bundles/Android/Package/" + version;
        }

        [MenuItem("工具/构建/补丁/EXE32")]
        public static void BuildPatchEXE32()
        {
            BuildScripts();
            var version = BuildAssetBundles(BuildTarget.StandaloneWindows);
            var path = Application.dataPath.Replace("/Assets", "") + "/Bundles/StandaloneWindows/Package/" + version;
        }

        [MenuItem("工具/构建/补丁/EXE64")]
        public static void BuildPatchEXE64()
        {
            BuildScripts();
            var version = BuildAssetBundles(BuildTarget.StandaloneWindows64);
            var path = Application.dataPath.Replace("/Assets", "") + "/Bundles/StandaloneWindows64/Package/" + version;
        }

        [MenuItem("工具/构建/构建资源")]
        public static void BuildAssetBundles()
        {
            BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        }

        private static string BuildAssetBundles(BuildTarget buildTarget)
        {
            Debug.Log($"构建平台目标 : {buildTarget}");

            var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            var version = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
            buildParameters.BuildOutputRoot = buildoutputRoot;
            buildParameters.BuildinFileRoot = streamingAssetsRoot;
            buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline.ToString();
            buildParameters.BuildTarget = buildTarget;
            buildParameters.BuildMode = EBuildMode.IncrementalBuild;
            buildParameters.PackageName = "Package";
            buildParameters.PackageVersion = version;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = EFileNameStyle.HashName;
            buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll;
            buildParameters.BuildinFileCopyParams = string.Empty;
            buildParameters.EncryptionServices = new EncryptionNone();
            buildParameters.CompressOption = ECompressOption.LZ4;

            BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {
                Debug.Log($"构建资源成功 : {buildResult.OutputPackageDirectory}");
            }
            else
            {
                Debug.LogError($"构建资源失败 : {buildResult.ErrorInfo}");
            }

            return version;
        }

        [MenuItem("工具/构建/构建代码")]
        public static void BuildScripts()
        {
            GenScripts();
            ScriptsCopy();
        }

        private static void GenScripts()
        {
#if HYBRID_CLR
            PrebuildCommand.GenerateAll();
#endif
        }

        private static void ScriptsCopy()
        {
#if HYBRID_CLR
            var rootPath = Application.dataPath.Replace("/Assets", "/");
            var scriptResPath = Application.dataPath + "/GameRes/Raws/Scripts/";
#if UNITY_STANDALONE_WIN
            var hotfixDLLPath = rootPath + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/StandaloneWindows64/";
            var aotDLLPath = rootPath + HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/StandaloneWindows64/";
#elif UNITY_ANDROID
            var hotfixDLLPath = rootPath + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/Android/";
            var aotDLLPath = rootPath + HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/Android/";
#elif UNITY_STANDALONE_OSX
            var hotfixDLLPath = rootPath + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/StandaloneWindows64/";
            var aotDLLPath = rootPath + HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/StandaloneWindows64/";
#elif UNITY_WEBGL
            var hotfixDLLPath = rootPath + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/WebGL/";
            var aotDLLPath = rootPath + HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/WebGL/";
#endif

            if (Directory.Exists(scriptResPath))
            {
                foreach (var filePath in Directory.GetFiles(scriptResPath)) File.Delete(filePath);
                Directory.Delete(scriptResPath);
            }

            Directory.CreateDirectory(scriptResPath);
            File.Copy($"{hotfixDLLPath}/Goblin.dll", $"{scriptResPath}/Goblin.dll.bytes");

            var filePaths = Directory.GetFiles(aotDLLPath);
            string fileNames = "";
            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                FileInfo fileInfo = new FileInfo(filePath);
                var fileName = fileInfo.Name;
                File.Copy(filePath, $"{scriptResPath}{fileName}.bytes");
                fileNames += fileName + (i < filePaths.Length - 1 ? "|" : "");
            }
            File.WriteAllText($"{scriptResPath}AOT_DLL_LIST.bytes", fileNames);

            Debug.Log("ScriptPackage Finished.");
            AssetDatabase.Refresh();
#endif
        }
    }
}
