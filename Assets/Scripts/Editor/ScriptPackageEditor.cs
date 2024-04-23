#if UNITY_EDITOR
using HybridCLR.Editor.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptPackageEditor : Editor
{
    [MenuItem("Package/ScriptPackage")]
    public static void ScriptPackage()
    {
        var rootPath = Application.dataPath.Replace("/Assets", "/");
        var scriptResPath = Application.dataPath + "/GameRawRes/Scripts/";
#if UNITY_STANDALONE_WIN
        var hotfixDLLPath = rootPath + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/StandaloneWindows64/";
        var aotDLLPath = rootPath + HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/StandaloneWindows64/";
#elif UNITY_ANDROID
        var hotfixDLLPath = rootPath + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/Android/";
        var aotDLLPath = rootPath + HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/Android/";
#elif  UNITY_STANDALONE_OSX
        var hotfixDLLPath = rootPath + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/StandaloneWindows64/";
        var aotDLLPath = rootPath + HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/StandaloneWindows64/";
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
    }
}
#endif