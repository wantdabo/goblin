using GoblinFramework.Render.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Render.GameResource
{
    /// <summary>
    /// 资源加载定位器，加载框架包装定位资源地址
    /// </summary>
    public class GameResLocation : CComp
    {
        private const string prefabPath = "GameRes/Prefabs/";
        private const string uiprefabPath = "GameRes/UIPrefabs/";
        private const string spritesPath = "GameRes/UISprites/";
        private const string configPath = "GameRawRes/Configs/";

        /// <summary>
        /// 异步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public async Task<GameObject> LoadUIPrefabAsync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(await engine.res.LoadAssetAsync<GameObject>(uiprefabPath + resName), parent ?? engine.ui.UIRoot.transform);
        }

        /// <summary>
        /// 同步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public GameObject LoadUIPrefabSync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(engine.res.LoadAssetSync<GameObject>(uiprefabPath + resName), parent ?? engine.ui.UIRoot.transform); 
        }

        /// <summary>
        /// 异步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public async Task<Sprite> LoadSpriteAsync(string resName)
        {
            return await engine.res.LoadAssetAsync<Sprite>(spritesPath + resName);
        }

        /// <summary>
        /// 同步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public Sprite LoadSpriteSync(string resName)
        {
            return engine.res.LoadAssetSync<Sprite>(spritesPath + resName);
        }
        
        /// <summary>
        /// 异步加载 Config 的 Bytes
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>RawBytes</returns>
        public async Task<byte[]> LoadConfigAsync(string resName)
        {
            return await engine.res.LoadRawFileAsync(configPath + resName);
        }
    }
}
