using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Common;
using UnityEngine;

namespace Goblin.GameResource
{
    /// <summary>
    /// 资源加载定位器，加载框架包装定位资源地址
    /// </summary>
    public class GameResLocation : Comp
    {
        public const string stagePath = "GameRes/Stages/";
        private const string modelPath = "GameRes/Models/";
        private const string effectPath = "GameRes/Effects/";
        private const string uieffectPath = "GameRes/UIEffects/";
        private const string uiprefabPath = "GameRes/UIPrefabs/";
        private const string spritesPath = "GameRes/UISprites/";
        private const string spAssetPath = "GameRes/SPAssets/";
        private const string espawnRulePath = "GameRes/ESpawnRules/";
        private const string espawnGroupDataPath = "GameRes/ESpawnRules/Groups/";
        private const string bloodDancePath = "GameRes/BloodDances/";
        private const string landscapePath = "GameRes/Landscapes/";
        private const string configPath = "GameRawRes/Configs/";

        /// <summary>
        /// 异步 Stage 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Stage 预制体</returns>
        public async Task<GameObject> LoadStageAsync(string resName)
        {
            return GameObject.Instantiate(await engine.gameRes.LoadAssetAsync<GameObject>(stagePath + resName));
        }

        /// <summary>
        /// 同步 Stage 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Stage 预制体</returns>
        public GameObject LoadStageSync(string resName)
        {
            return GameObject.Instantiate(engine.gameRes.LoadAssetSync<GameObject>(stagePath + resName));
        }

        /// <summary>
        /// 异步 Model 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Model 预制体</returns>
        public async Task<GameObject> LoadModelAsync(string resName)
        {
            return GameObject.Instantiate(await engine.gameRes.LoadAssetAsync<GameObject>(modelPath + resName));
        }

        /// <summary>
        /// 同步 Model 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Model 预制体</returns>
        public GameObject LoadModelSync(string resName)
        {
            return GameObject.Instantiate(engine.gameRes.LoadAssetSync<GameObject>(modelPath + resName));
        }

        /// <summary>
        /// 异步 Effect 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Effect 预制体</returns>
        public async Task<GameObject> LoadEffectAsync(string resName)
        {
            return GameObject.Instantiate(await engine.gameRes.LoadAssetAsync<GameObject>(effectPath + resName));
        }

        /// <summary>
        /// 同步 Effect 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Effect 预制体</returns>
        public GameObject LoadEffectSync(string resName)
        {
            return GameObject.Instantiate(engine.gameRes.LoadAssetSync<GameObject>(effectPath + resName));
        }

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
            try
            {
                GameObject prefab = await engine.gameRes.LoadAssetAsync<GameObject>(uiprefabPath + resName);
                if(prefab != null)
                {
                    Transform parentTransform = parent ?? engine.gameui.uiroot.transform;
                    if(parentTransform != null)
                    {
                        return GameObject.Instantiate(prefab, parentTransform);
                    }
                    else
                    {
                        Debug.LogError("Parent transform is null.");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load prefab: {uiprefabPath + resName}");
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Exception in LoadUIPrefabAsync: {ex.Message}");
            }

            return null;
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
