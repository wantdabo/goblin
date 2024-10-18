using Goblin.Core;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Common.GameRes
{
    /// <summary>
    /// 资源加载定位器，加载框架包装定位资源地址
    /// </summary>
    public class Location : Comp
    {
        public const string modelPath = "Assets/GameRes/Models/";
        private const string effectPath = "Assets/GameRes/Effects/";
        public const string uieffectPath = "Assets/GameRes/UIEffects/";
        public const string uiprefabPath = "Assets/GameRes/UIPrefabs/";
        public const string spritesPath = "Assets/GameRes/UISprites/";
        public const string configPath = "Assets/GameRes/Raws/Configs/";
        public const string skilldataPath = "Assets/GameRes/Raws/SkillDatas/";

        /// <summary>
        /// 异步 Model 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Model 预制体</returns>
        public async Task<GameObject> LoadModelAsync(string resName)
        {
            return GameObject.Instantiate(await engine.gameres.LoadAssetAsync<GameObject>(modelPath + resName));
        }

        /// <summary>
        /// 同步 Model 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Model 预制体</returns>
        public GameObject LoadModelSync(string resName)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(modelPath + resName));
        }
        
        /// <summary>
        /// 异步 Effect 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Effect 预制体</returns>
        public async Task<GameObject> LoadEffectAsync(string resName)
        {
            return GameObject.Instantiate(await engine.gameres.LoadAssetAsync<GameObject>(effectPath + resName));
        }

        /// <summary>
        /// 同步 Effect 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Effect 预制体</returns>
        public GameObject LoadEffectSync(string resName)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(effectPath + resName));
        }

        /// <summary>
        /// 同步 UIEffect 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>UIEffect 预制体</returns>
        public GameObject LoadUIEffectSync(string resName)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(uieffectPath + resName));
        }

        /// <summary>
        /// 异步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public async Task<GameObject> LoadUIPrefabAsync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(await engine.gameres.LoadAssetAsync<GameObject>(uiprefabPath + resName), parent ?? engine.gameui.uiroot.transform);
        }


        /// <summary>
        /// 同步 UI 加载预制体
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public GameObject LoadUIPrefabSync(string resName, Transform parent = null)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(uiprefabPath + resName), parent ?? engine.gameui.uiroot.transform);
        }

        /// <summary>
        /// 异步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public async Task<Sprite> LoadSpriteAsync(string resName)
        {
            return await engine.gameres.LoadAssetAsync<Sprite>(spritesPath + resName);
        }

        /// <summary>
        /// 同步加载 Sprite
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public Sprite LoadSpriteSync(string resName)
        {
            return engine.gameres.LoadAssetSync<Sprite>(spritesPath + resName);
        }

        /// <summary>
        /// 同步加载 Config 的 Bytes
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>RawBytes</returns>
        public byte[] LoadConfigSync(string resName)
        {
            return engine.gameres.LoadRawFileSync(configPath + resName);
        }

        /// <summary>
        /// 异步加载 Config 的 Bytes
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>RawBytes</returns>
        public async Task<byte[]> LoadConfigAsync(string resName)
        {
            return await engine.gameres.LoadRawFileAsync(configPath + resName);
        }
        
        /// <summary>
        /// 同步加载 SkillData 的 Bytes
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>RawBytes</returns>
        public byte[] LoadSkillDataSync(string resName)
        {
            return engine.gameres.LoadRawFileSync(skilldataPath + resName);
        }
        
        /// <summary>
        /// 异步加载 SkillData 的 Bytes
        /// </summary>
        /// <param name="resName">资源地址</param>
        /// <returns>RawBytes</returns>
        public async Task<byte[]> LoadSkillDataAsync(string resName)
        {
            return await engine.gameres.LoadRawFileAsync(skilldataPath + resName);
        }
    }
}
