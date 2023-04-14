﻿using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using GoblinFramework.Render.Common;
using GoblinFramework.Core;

namespace GoblinFramework.Render.GameResource
{
    /// <summary>
    /// Game-Resources-Comp 资源加载组件
    /// </summary>
    public abstract class GameRes : RComp
    {
        /// <summary>
        /// 资源加载定位器，具体的加载在这里实现
        /// </summary>
        public GameResLocation location;

        protected override void OnCreate()
        {
            base.OnCreate();
            location = AddComp<GameResLocation>();
            location.Create();
        }

        /// <summary>
        /// 异步初始化资源组件
        /// </summary>
        /// <returns>Task</returns>
        public abstract Task InitialGameRes();

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
