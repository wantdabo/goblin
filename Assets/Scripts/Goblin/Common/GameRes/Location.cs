using Goblin.Core;
using System.Threading.Tasks;
using Goblin.Gameplay.Render.Common;
using UnityEngine;

namespace Goblin.Common.GameRes
{
    /// <summary>
    /// 资源加载定位器，加载框架包装定位资源地址
    /// </summary>
    public class Location : Comp
    {
        public const string soundpath = "Assets/GameRes/Sound/";
        public const string modelpath = "Assets/GameRes/Model/";
        public const string effectpath = "Assets/GameRes/Effect/";
        public const string uieffectpath = "Assets/GameRes/UIEffect/";
        public const string uiprefabpath = "Assets/GameRes/UIPrefab/";
        public const string spritespath = "Assets/GameRes/UISprite/";
        public const string configpath = "Assets/GameRes/Raw/Configs/";
        public const string pipelinepath = "Assets/GameRes/Raw/Pipelines/";

        /// <summary>
        /// 异步加载音效预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>音效预制体</returns>
        public async Task<GameObject> LoadSoundAsync(string res)
        {
            return GameObject.Instantiate(await engine.gameres.LoadAssetAsync<GameObject>(soundpath + res));
        }

        /// <summary>
        /// 同步加载音效预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>音效预制体</returns>
        public GameObject LoadSoundSync(string res)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(soundpath + res));
        }
        
        /// <summary>
        /// 异步 Model 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Model 预制体</returns>
        public async Task<GameObject> LoadModelAsync(string res)
        {
            return GameObject.Instantiate(await engine.gameres.LoadAssetAsync<GameObject>(modelpath + res));
        }

        /// <summary>
        /// 同步 Model 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Model 预制体</returns>
        public GameObject LoadModelSync(string res)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(modelpath + res));
        }
        
        /// <summary>
        /// 异步加载模型动画配置
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>AnimationConfig</returns>
        public async Task<AnimationConfig> LoadModelAnimationConfigAsync(string res)
        {
            return await engine.gameres.LoadAssetAsync<AnimationConfig>(modelpath + res);
        }
        
        /// <summary>
        /// 同步加载模型动画配置
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>AnimationConfig</returns>
        public AnimationConfig LoadModelAnimationConfigSync(string res)
        {
            return engine.gameres.LoadAssetSync<AnimationConfig>(modelpath + res);
        }
        
        /// <summary>
        /// 异步 Effect 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Effect 预制体</returns>
        public async Task<GameObject> LoadEffectAsync(string res)
        {
            return GameObject.Instantiate(await engine.gameres.LoadAssetAsync<GameObject>(effectpath + res));
        }

        /// <summary>
        /// 同步 Effect 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Effect 预制体</returns>
        public GameObject LoadEffectSync(string res)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(effectpath + res));
        }

        /// <summary>
        /// 同步 UIEffect 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>UIEffect 预制体</returns>
        public GameObject LoadUIEffectSync(string res)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(uieffectpath + res));
        }

        /// <summary>
        /// 异步 UI 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public async Task<GameObject> LoadUIPrefabAsync(string res, Transform parent = null)
        {
            return GameObject.Instantiate(await engine.gameres.LoadAssetAsync<GameObject>(uiprefabpath + res), parent ?? engine.gameui.uiroot.transform);
        }

        /// <summary>
        /// 同步 UI 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <param name="parent">挂载的父节点</param>
        /// <returns>UIPrefab UI 预制体</returns>
        public GameObject LoadUIPrefabSync(string res, Transform parent = null)
        {
            return GameObject.Instantiate(engine.gameres.LoadAssetSync<GameObject>(uiprefabpath + res), parent ?? engine.gameui.uiroot.transform);
        }

        /// <summary>
        /// 异步加载 Sprite
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public async Task<Sprite> LoadSpriteAsync(string res)
        {
            return await engine.gameres.LoadAssetAsync<Sprite>(spritespath + res);
        }

        /// <summary>
        /// 同步加载 Sprite
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Sprite 精灵</returns>
        public Sprite LoadSpriteSync(string res)
        {
            return engine.gameres.LoadAssetSync<Sprite>(spritespath + res);
        }

        /// <summary>
        /// 同步加载 Config 的 Bytes
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>RawBytes</returns>
        public byte[] LoadConfigSync(string res)
        {
            return engine.gameres.LoadRawFileSync(configpath + res);
        }

        /// <summary>
        /// 异步加载 Config 的 Bytes
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>RawBytes</returns>
        public async Task<byte[]> LoadConfigAsync(string res)
        {
            return await engine.gameres.LoadRawFileAsync(configpath + res);
        }
        
        /// <summary>
        /// 同步加载 PipelineData 的 Bytes
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>RawBytes</returns>
        public byte[] LoadPipelineSync(string res)
        {
            return engine.gameres.LoadRawFileSync(pipelinepath + res);
        }

        /// <summary>
        /// 异步加载 PipelineData 的 Bytes
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>RawBytes</returns>
        public async Task<byte[]> LoadPipelineAsync(string res)
        {
            return await engine.gameres.LoadRawFileAsync(pipelinepath + res);
        }
    }
}
