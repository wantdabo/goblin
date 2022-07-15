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
    public class YooGameResComp : GameResComp
    {

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override Task<T> LoadAssetAsync<T>(string resName)
        {
            throw new NotImplementedException();
        }

        public override T LoadAssetSync<T>(string resName)
        {
            throw new NotImplementedException();
        }

        public override Task<byte[]> LoadRawFileAsync(string resName)
        {
            throw new NotImplementedException();
        }

        public override Task<Scene> LoadSceneASync(string resName)
        {
            throw new NotImplementedException();
        }
    }
}
