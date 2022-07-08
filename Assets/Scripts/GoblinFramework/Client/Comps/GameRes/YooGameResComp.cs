using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace GoblinFramework.Client.Comps.GameRes
{
    /// <summary>
    /// YooAsset 资源加载组件
    /// </summary>
    internal class YooGameResComp : GameResComp
    {
        internal override void LoadAssetAsync<T>(string resName, Action<T> callback)
        {
            throw new NotImplementedException();
        }

        internal override void LoadAssetSync<T>(string resName, Action<T> callback)
        {
            throw new NotImplementedException();
        }

        internal override void LoadRawFileAsync(string resName, Action<byte[]> callback)
        {
            throw new NotImplementedException();
        }

        internal override void LoadSceneASync(string resName, Action<Scene> callback)
        {
            throw new NotImplementedException();
        }
    }
}
