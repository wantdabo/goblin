using UnityEngine;
using YooAsset;

/// <summary>
/// Զ����Դ��ַ��ѯ������
/// </summary>
public class RemoteServices : IRemoteServices
{
    private string remoteHost = null;

    private string GetHostServer()
    {
        if (null != remoteHost) return remoteHost;

        var handle = YooAssets.LoadAssetSync<TextAsset>("Assets/GameRes/Raws/YA_REMOTE_INFO");
        var ta = handle.AssetObject as TextAsset;
        handle.Release();
        remoteHost = ta.text;

        return remoteHost;
    }

    string IRemoteServices.GetRemoteMainURL(string fileName)
    {
        return $"{GetHostServer()}/{fileName}";
    }

    string IRemoteServices.GetRemoteFallbackURL(string fileName)
    {
        return $"{GetHostServer()}/{fileName}";
    }
}