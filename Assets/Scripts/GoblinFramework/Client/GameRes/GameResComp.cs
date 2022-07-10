using GoblinFramework.Core;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Comps.GameRes
{
    internal abstract class GameResComp : ClientComp
    {
        internal abstract void LoadAssetAsync<T>(string resName, Action<T> callback) where T : UnityEngine.Object;
        internal abstract void LoadAssetSync<T>(string resName, Action<T> callback) where T : UnityEngine.Object;
        internal abstract void LoadRawFileAsync(string resName, Action<byte[]> callback);
        internal abstract void LoadSceneASync(string resName, Action<Scene> callback);
    }
}
