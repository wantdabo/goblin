using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class GameResHelper
{
    private static string scriptsPath = "Assets/GameRawRes/Scripts/";

    public static TextAsset LoadTextAssetSync(string resName)
    {
        var handle = YooAssets.LoadAssetSync<TextAsset>(scriptsPath + resName);
        var ta = handle.AssetObject as TextAsset;
        handle.Release();

        return ta;
    }

    public static async Task<bool> UpdateRes()
    {
        var package = YooAssets.GetPackage("Package");
        var PackageVersionOperation = package.UpdatePackageVersionAsync();
        await PackageVersionOperation.Task;

        if (PackageVersionOperation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            string packageVersion = PackageVersionOperation.PackageVersion;
            Debug.Log($"Updated package Version : {packageVersion}");

            // 更新成功后自动保存版本号，作为下次初始化的版本。
            // 也可以通过operation.SavePackageVersion()方法保存。
            bool savePackageVersion = true;
            package = YooAssets.GetPackage("Package");
            var PackageMannifestOperation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
            await PackageMannifestOperation.Task;

            if (PackageMannifestOperation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                package = YooAssets.GetPackage("Package");

                var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

                //没有需要下载的资源
                if (downloader.TotalDownloadCount == 0)
                {
                    return false;
                }

                //需要下载的文件总数和总大小
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                Debug.Log($"需要下载的文件总数和总大小 ===============> {totalDownloadCount}, {totalDownloadBytes}");
                //开启下载
                downloader.BeginDownload();
                await downloader.Task;

                //检测下载结果
                if (downloader.Status == EOperationStatus.Succeed)
                {
                    //下载成功
                    return true;
                }
                else
                {
                    //下载失败

                    return false;
                }
            }
            else
            {
                //更新失败
                Debug.LogError(PackageMannifestOperation.Error);

                return false;
            }
        }
        else
        {
            //更新失败
            Debug.LogError(PackageVersionOperation.Error);

            return false;
        }
    }
}
