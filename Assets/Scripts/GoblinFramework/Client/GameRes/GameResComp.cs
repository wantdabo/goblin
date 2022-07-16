using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.GameRes
{
    public abstract class GameResComp : ClientComp
    {
        /// <summary>
        /// 资源加载定位器，具体的加载在这里实现
        /// </summary>
        public GameResLocationComp Location;

        protected override void OnCreate()
        {
            base.OnCreate();
            Location = AddComp<GameResLocationComp>();
        }

        public abstract Task<T> LoadAssetAsync<T>(string resName) where T : Object;
        public abstract T LoadAssetSync<T>(string resName) where T : Object;
        public abstract Task<byte[]> LoadRawFileAsync(string resName);
        public abstract Task<Scene> LoadSceneASync(string resName, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
    }
}
