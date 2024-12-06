using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Sys.Common
{
    /// <summary>
    /// UI 界面，基础类
    /// </summary>
    public abstract class UIBaseView : UIBase<UIBaseView>
    {
        /// <summary>
        /// UILayer 层级
        /// </summary>
        public abstract UILayer layer { get; }
        /// <summary>
        /// UI 状态
        /// </summary>
        public UIState state { get; private set; }
        /// <summary>
        /// 支持快速关闭
        /// </summary>
        public virtual bool quickclose { get; } = true;
        /// <summary>
        /// Canvas
        /// </summary>
        private Canvas canvas;

        private string mLayerName;
        /// <summary>
        /// Layer 名字
        /// </summary>
        public override string layerName
        {
            get
            {
                return mLayerName;
            }
            set
            {
                mLayerName = value;
                if (null == canvas) return;
                canvas.sortingLayerName = mLayerName;
            }
        }

        private int mSorting = 0;
        /// <summary>
        /// Sorting 值
        /// </summary>
        public override int sorting
        {
            get
            {
                return mSorting;
            }
            set
            {
                mSorting = value;
                if (null == canvas) return;
                canvas.sortingOrder = mSorting;
            }
        }

        /// <summary>
        /// 参数
        /// </summary>
        protected object[] args;

        /// <summary>
        /// 加载 UI
        /// </summary>
        /// <returns>返回 UI</returns>
        public async Task<UIBaseView> Load()
        {
            state = UIState.Loading;
            gameObject = await engine.gameres.location.LoadUIPrefabAsync(res, engine.gameui.GetLayerNode(layer).transform);

            canvas = gameObject.GetComponent<Canvas>();
            sorting = mSorting;
            layerName = mLayerName;

            OnLoad();
            OnBuildUI();
            OnBindEvent();

            state = UIState.Loaded;

            return this;
        }

        /// <summary>
        /// 打开 UI
        /// </summary>
        public void Open(params object[] args)
        {
            this.args = args;
            if (UIState.Loading == state) return;
            if (UIState.Open == state) Close();

            layerName = layer.ToString();
            sorting = engine.gameui.AllotSorting();

            OnOpen();
        }

        /// <summary>
        /// 卸载 UI 回调
        /// </summary>
        protected override void OnUnload()
        {
            state = UIState.Free;
            base.OnUnload();
        }

        /// <summary>
        /// 打开 UI 回调
        /// </summary>
        protected override void OnOpen()
        {
            state = UIState.Open;
            base.OnOpen();
        }

        /// <summary>
        /// 关闭 UI 回调
        /// </summary>
        protected override void OnClose()
        {
            state = UIState.Close;
            base.OnClose();
            engine.gameui.Close(GetType());
        }
    }
}
