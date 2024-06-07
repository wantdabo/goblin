using System.IO;
using UnityEngine;

using YooAsset;

/// <summary>
/// ��Դ�ļ������ؽ�����
/// </summary>
public class FileStreamDecryption : IDecryptionServices
{
    /// <summary>
    /// ͬ����ʽ��ȡ���ܵ���Դ������
    /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
    /// </summary>
    AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
    {
        BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        managedStream = bundleStream;
        return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
    }

    /// <summary>
    /// �첽��ʽ��ȡ���ܵ���Դ������
    /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
    /// </summary>
    AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
    {
        BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        managedStream = bundleStream;
        return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
    }

    private static uint GetManagedReadBufferSize()
    {
        return 1024;
    }
}