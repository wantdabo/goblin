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
        public abstract UILayer layer { get; }

        private UIState mState = UIState.Free;
        public UIState state { get { return mState; } private set { mState = value; } }

        private Canvas canvas;

        private string mLayerName;
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
        public void Open()
        {
            if (UIState.Loading == state) return;
            if (UIState.Open == state) Close();

            layerName = layer.ToString();
            sorting = engine.gameui.AllotSorting();

            OnOpen();
        }

        protected override void OnUnload()
        {
            state = UIState.Free;
        }

        protected override void OnOpen()
        {
            state = UIState.Open;
        }

        protected override void OnClose()
        {
            state = UIState.Close;
        }
    }
}
