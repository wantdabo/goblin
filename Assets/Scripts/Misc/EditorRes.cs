using Goblin.Common.GameRes;
using Goblin.Gameplay.Render.Common;
using UnityEditor;
using UnityEngine;

namespace Goblin.Misc
{
    /// <summary>
    /// 编辑器资源加载器
    /// </summary>
    public class EditorRes
    {
        /// <summary>
        /// 同步 Model 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Model 预制体</returns>
        public static GameObject LoadModel(string res)
        {
            return GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Location.modelpath + res + ".prefab"));
        }

        /// <summary>
        /// 同步 Effect 加载预制体
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>Effect 预制体</returns>
        public static GameObject LoadEffect(string res)
        {
            return GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Location.effectpath + res + ".prefab"));
        }

        /// <summary>
        /// 加载模型配置
        /// </summary>
        /// <param name="res">资源地址</param>
        /// <returns>AnimationConfig</returns>
        public static AnimationConfig LoadAnimationConfig(string res)
        {
            return ScriptableObject.Instantiate(AssetDatabase.LoadAssetAtPath<AnimationConfig>(Location.animcfgpath + res + ".asset"));
        }
    }
}