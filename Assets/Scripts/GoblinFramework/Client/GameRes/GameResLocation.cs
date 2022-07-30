using GoblinFramework.Client.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.GameResource
{
    /// <summary>
    /// 资源加载定位器，加载框架包装定位资源地址
    /// </summary>
    public class GameResLocation : CComp
    {
        private const string actorsPath = "Actors/";
        private const string uiprefabPath = "UIPrefabs/";
        private const string spritesPath = "UISprites/";

        /// <summary>
        /// 异步 Actor 加载预制体
        /// </summary>
        /// <param name="resName"></param>
        /// <returns>Actor GameObject</returns>
        public async Task<GameObject> LoadActorPrefabAsync(string resName) 
        {
            return GameObject.Instantiate(await Engine.GameRes.LoadAssetAsync<GameObject>(actorsPath + resName));
        }
        /// <summary>
        /// 同步 Actor 加载预制体
        /// </summary>
        /// <param name="resName"></param>
        /// <returns>Actor GameObject</returns>

        public GameObject LoadActorPrefabSync(string resName) 
        {
            return GameObject.Instantiate(Engine.GameRes.LoadAssetSync<GameObject>(actorsPath + resName));
        }

        /// <summary>
        /// 异步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public async Task<GameObject> LoadUIPrefabAsync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(await Engine.GameRes.LoadAssetAsync<GameObject>(uiprefabPath + resName), parent ?? Engine.GameUI.UIRoot.transform);
        }

        /// <summary>
        /// 同步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public GameObject LoadUIPrefabSync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(Engine.GameRes.LoadAssetSync<GameObject>(uiprefabPath + resName), parent ?? Engine.GameUI.UIRoot.transform); 
        }

        /// <summary>
        /// 异步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public async Task<Sprite> LoadSpriteAsync(string resName)
        {
            return await Engine.GameRes.LoadAssetAsync<Sprite>(spritesPath + resName);
        }

        /// <summary>
        /// 同步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public Sprite LoadSpriteSync(string resName)
        {
            return Engine.GameRes.LoadAssetSync<Sprite>(spritesPath + resName);
        }
    }
}
