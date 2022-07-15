using GoblinFramework.Core;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Comps.GameRes
{
    public abstract class GameResComp : ClientComp
    {
        public abstract Task<T> LoadAssetAsync<T>(string resName) where T : UnityEngine.Object;
        public abstract T LoadAssetSync<T>(string resName) where T : UnityEngine.Object;
        public abstract Task<Scene> LoadSceneASync(string resName, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
    }
}
