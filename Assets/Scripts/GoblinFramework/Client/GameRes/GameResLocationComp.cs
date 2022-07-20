using GoblinFramework.Client.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.GameRes
{
    public class GameResLocationComp : Comp<CGEngineComp>
    {
        private const string prefabsPath = "UIPrefabs/";
        private const string spritesPath = "UISprites/";

        /// <summary>
        /// 异步加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>GameObject</returns>
        public async Task<GameObject> LoadUIPrefabAsync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(await Engine.GameRes.LoadAssetAsync<GameObject>(prefabsPath + resName), parent ?? Engine.GameUI.UIRoot.transform);
        }

        /// <summary>
        /// 同步加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>GameObject</returns>
        public GameObject LoadUIPrefabSync(string resName)
        {
            return Engine.GameRes.LoadAssetSync<GameObject>(prefabsPath + resName);
        }

        /// <summary>
        /// 异步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite</returns>
        public async Task<Sprite> LoadSpriteAsync(string resName)
        {
            return await Engine.GameRes.LoadAssetAsync<Sprite>(spritesPath + resName);
        }

        /// <summary>
        /// 同步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite</returns>
        public Sprite LoadSpriteSync(string resName)
        {
            return Engine.GameRes.LoadAssetSync<Sprite>(spritesPath + resName);
        }
    }
}
