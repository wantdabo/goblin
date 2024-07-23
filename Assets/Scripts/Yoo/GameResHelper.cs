using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class GameResHelper
{
    private static string scriptsPath = "Assets/GameRes/Raws/Scripts/";

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
            //���³ɹ�
            string packageVersion = PackageVersionOperation.PackageVersion;
            Debug.Log($"Updated package Version : {packageVersion}");

            // ���³ɹ����Զ�����汾�ţ���Ϊ�´γ�ʼ���İ汾��
            // Ҳ����ͨ��operation.SavePackageVersion()�������档
            bool savePackageVersion = true;
            package = YooAssets.GetPackage("Package");
            var PackageMannifestOperation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
            await PackageMannifestOperation.Task;

            if (PackageMannifestOperation.Status == EOperationStatus.Succeed)
            {
                //���³ɹ�
                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                package = YooAssets.GetPackage("Package");

                var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

                //û����Ҫ���ص���Դ
                if (downloader.TotalDownloadCount == 0)
                {
                    return false;
                }

                //��Ҫ���ص��ļ��������ܴ�С
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                Debug.Log($"��Ҫ���ص��ļ��������ܴ�С ===============> {totalDownloadCount}, {totalDownloadBytes}");
                //��������
                downloader.BeginDownload();
                await downloader.Task;

                //������ؽ��
                if (downloader.Status == EOperationStatus.Succeed)
                {
                    //���سɹ�
                    return true;
                }
                else
                {
                    //����ʧ��

                    return false;
                }
            }
            else
            {
                //����ʧ��
                Debug.LogError(PackageMannifestOperation.Error);

                return false;
            }
        }
        else
        {
            //����ʧ��
            Debug.LogError(PackageVersionOperation.Error);

            return false;
        }
    }
}
