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
    public abstract class UIBaseView : UIBase
    {
        public abstract UILayer UILayer { get; }

        private UIState uiState = UIState.Free;
        public UIState UIState { get { return uiState; } private set { uiState = value; } }

        private int sorting = 0;
        public int Sorting
        {
            get
            {
                return sorting;
            }
            set
            {
                sorting = value;
                if (null == canvas) return;
                canvas.sortingOrder = sorting;
            }
        }

        private Canvas canvas;

        public override async void Load()
        {
            UIState = UIState.Loading;
            gameObject = await Engine.GameRes.Location.LoadUIPrefabAsync(UIRes, Engine.GameUI.GetLayerNode(UILayer).transform);

            canvas = gameObject.GetComponent<Canvas>();
            Sorting = sorting;

            OnBuildUI();
            OnBindEvent();

            base.Load();
        }

        public override void Unload()
        {
            UIState = UIState.Free;
            base.Unload();
        }

        public override void Open()
        {
            if (null == gameObject) Load();
            UIState = UIState.Open;
            base.Open();
        }

        public override void Close()
        {
            UIState = UIState.Close;
            base.Close();
            Unload();
        }
    }
}
