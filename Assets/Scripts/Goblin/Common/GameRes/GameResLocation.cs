using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Common;
using UnityEngine;

namespace Goblin.Common.Res
{
    /// <summary>
    /// 资源加载定位器，加载框架包装定位资源地址
    /// </summary>
    public class GameResLocation : Comp
    {
        private const string uieffectPath = "Assets/GameRes/UIEffects/";
        private const string uiprefabPath = "Assets/GameRes/UIPrefabs/";
        private const string spritesPath = "Assets/GameRes/UISprites/";
        private const string configPath = "Assets/GameRawRes/Configs/";

        /// <summary>
        /// 同步 UIEffect 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>UIEffect 预制体</returns>
        public GameObject LoadUIEffectSync(string resName) 
        {
            return GameObject.Instantiate(engine.gameRes.LoadAssetSync<GameObject>(uieffectPath + resName));
        }

        /// <summary>
        /// 异步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public async Task<GameObject> LoadUIPrefabAsync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(await engine.gameRes.LoadAssetAsync<GameObject>(uiprefabPath + resName), parent ?? engine.gameui.uiroot.transform);
        }


        /// <summary>
        /// 同步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public GameObject LoadUIPrefabSync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(engine.gameRes.LoadAssetSync<GameObject>(uiprefabPath + resName), parent ?? engine.gameui.uiroot.transform);
        }

        /// <summary>
        /// 异步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public async Task<Sprite> LoadSpriteAsync(string resName)
        {
            return await engine.gameRes.LoadAssetAsync<Sprite>(spritesPath + resName);
        }

        /// <summary>
        /// 同步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public Sprite LoadSpriteSync(string resName)
        {
            return engine.gameRes.LoadAssetSync<Sprite>(spritesPath + resName);
        }

        /// <summary>
        /// 同步加载 Config 的 Bytes
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>RawBytes</returns>
        public byte[] LoadConfigSync(string resName)
        {
            return engine.gameRes.LoadRawFileSync(configPath + resName);
        }

        /// <summary>
        /// 异步加载 Config 的 Bytes
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>RawBytes</returns>
        public async Task<byte[]> LoadConfigAsync(string resName)
        {
            return await engine.gameRes.LoadRawFileAsync(configPath + resName);
        }
    }
}
