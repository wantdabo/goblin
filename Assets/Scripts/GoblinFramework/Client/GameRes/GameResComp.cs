using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using GoblinFramework.Client.Common;
using GoblinFramework.Core;

namespace GoblinFramework.Client.GameRes
{
    /// <summary>
    /// Game-Resources-Comp 资源加载组件
    /// </summary>
    public abstract class GameResComp : Comp<CGEngineComp>
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

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resName">资源名</param>
        /// <returns>资源</returns>
        public abstract Task<T> LoadAssetAsync<T>(string resName) where T : Object;
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resName">资源名</param>
        /// <returns>资源</returns>
        public abstract T LoadAssetSync<T>(string resName) where T : Object;
        /// <summary>
        /// 异步加载二进制资源
        /// </summary>
        /// <param name="resName">资源名</param>
        /// <returns>二进制资源</returns>
        public abstract Task<byte[]> LoadRawFileAsync(string resName);
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="resName">场景名</param>
        /// <returns>场景</returns>
        public abstract Task<Scene> LoadSceneASync(string resName, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
    }
}
