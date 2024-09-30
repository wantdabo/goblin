using Goblin.Common;
using Goblin.Gameplay.Common;
using Goblin.Gameplay.Core;
using System.Collections;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 模型加载状态枚举
    /// </summary>
    public enum LoadModelStatus
    {
        /// <summary>
        /// 开始
        /// </summary>
        Start,
        /// <summary>
        /// 结束
        /// </summary>
        End,
    }

    /// <summary>
    /// 根据运动来翻转模型
    /// </summary>
    public struct SettingsMotionReverseEvent : IEvent
    {
        /// <summary>
        /// 运动量
        /// </summary>
        public Vector2 motion;
    }

    /// <summary>
    /// 加载模型事件
    /// </summary>
    public struct LoadModelEvent : IEvent
    {
        /// <summary>
        /// 模型资源名字
        /// </summary>
        public string res;
    }

    /// <summary>
    /// 模型加载状态事件
    /// </summary>
    public struct LoadModelStatusChangedEvent : IEvent
    {
        /// <summary>
        /// 模型加载状态
        /// </summary>
        public LoadModelStatus status;
    }

    /// <summary>
    /// 模型
    /// </summary>
    public class ModelRender : Behavior
    {
        /// <summary>
        /// 模型资源名
        /// </summary>
        public string modelName { get; private set; }

        /// <summary>
        /// 模型的 GameObject
        /// </summary>
        public GameObject model { get; private set; }

        /// <summary>
        /// 模型的 Transform
        /// </summary>
        private Transform trans { get; set; }

        /// <summary>
        /// 模型回收的存放处
        /// </summary>
        private static GameObject modelPool = new("MODELSPOOL");

        static ModelRender()
        {
            modelPool.SetActive(false);
        }

        private Node node;

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<LoadModelEvent>(OnLoadModel);
            actor.eventor.Listen<SettingsMotionReverseEvent>(OnSettingsMotionReverse);
            node = actor.GetBehavior<Node>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.eventor.UnListen<LoadModelEvent>(OnLoadModel);
            actor.eventor.UnListen<SettingsMotionReverseEvent>(OnSettingsMotionReverse);
            node = null;
            if (null != model) engine.pool.Set($"GAME_MODEL_RENDER_KEY{modelName}", model, (go) => go.transform.SetParent(modelPool.transform));
        }

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="e">加载模型的参数</param>
        private void OnLoadModel(LoadModelEvent e)
        {
            if (null != modelName && modelName.Equals(e.res)) return;

            if (null != model) engine.pool.Set($"GAME_MODEL_RENDER_KEY{modelName}", model, (go) => go.transform.SetParent(modelPool.transform));

            modelName = e.res;
            actor.eventor.Tell(new LoadModelStatusChangedEvent { status = LoadModelStatus.Start });

            model = engine.pool.Get<GameObject>($"GAME_MODEL_RENDER_KEY{modelName}");
            if (null == model) model = engine.gameres.location.LoadModelSync(modelName);
            trans = model.GetComponent<Transform>();
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            if (null == node || null == node.gameObject)
            {
                engine.pool.Set($"GAME_MODEL_RENDER_KEY{modelName}", model, (go) => go.transform.SetParent(modelPool.transform));
                return;
            }

            if (null != node && null != node.gameObject) model.transform.SetParent(node.gameObject.transform, false);
            actor.eventor.Tell(new LoadModelStatusChangedEvent { status = LoadModelStatus.End });
        }


        private static Vector3 reverseScale = new Vector3(-1, 1, 1);
        private void OnSettingsMotionReverse(SettingsMotionReverseEvent e)
        {
            if (null == trans) return;

            trans.localScale = e.motion.x < 0 ? reverseScale : Vector3.one;
        }
    }
}