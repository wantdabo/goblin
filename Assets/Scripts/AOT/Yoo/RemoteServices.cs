using System.IO;
using UnityEngine;
using YooAsset;

namespace AOT.Yoo
{
    /// <summary>
    /// CDN 地址
    /// </summary>
    public class RemoteServices : IRemoteServices
    {
        private readonly string iosRemoteHost = "http://192.168.2.146/CDN/IOS";
        private readonly string androidRemoteHost = "http://192.168.2.146/CDN/Android";
        private readonly string webglRemoteHost = "http://192.168.2.146/CDN/WebGL";

        private string GetHostServer()
        {
#if UNITY_EDITOR || UNITY_EDITOR_OSX
            return string.Empty;
#elif UNITY_IOS
        return iosRemoteHost;
#elif UNITY_ANDROID
        return androidRemoteHost;
#elif UNITY_WEBGL
        return webglRemoteHost;
#endif
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
}
