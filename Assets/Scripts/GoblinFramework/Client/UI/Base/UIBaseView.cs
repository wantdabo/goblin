using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GoblinFramework.Client.UI.GameUI;

namespace GoblinFramework.Client.UI.Base
{
    /// <summary>
    /// UI 界面，基础类
    /// </summary>
    public abstract class UIBaseView : UIBase<UIBaseView>
    {
        public abstract UILayer UILayer { get; }

        private UIState mState = UIState.Free;
        public UIState state { get { return mState; } private set { mState = value; } }

        private int mSorting = 0;
        public int sorting
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

        private Canvas canvas;

        public override async Task<UIBaseView> Load()
        {
            state = UIState.Loading;
            gameObject = await Engine.GameRes.Location.LoadUIPrefabAsync(UIRes, Engine.GameUI.GetLayerNode(UILayer).transform);

            canvas = gameObject.GetComponent<Canvas>();
            sorting = mSorting;
            
            OnLoad();
            OnBuildUI();
            OnBindEvent();

            return this;
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
