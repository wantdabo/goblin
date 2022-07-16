using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GoblinFramework.Client.UI.GameUI;

namespace GoblinFramework.Client.UI
{
    public abstract class UIBaseView : UIBase
    {
        private UIState uiState = UIState.Free;
        private int sorting = 0;

        public UIState UIState { get { return uiState; } private set { uiState = value; } }
        public abstract UILayer UILayer { get; }
        public int Sorting { get { return sorting; } set { sorting = value; } }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIState = UIState.Free;
        }

        public async void Open()
        {
            if (null == gameObject)
            {
                UIState = UIState.Loading;
                gameObject = await Engine.GameRes.Location.LoadUIPrefabAsync(UIName);
                gameObject.layer = LayerMask.NameToLayer(UILayer.ToString());

                OnBuildUI();
                OnBindEvent();
            }

            OnOpen();
            UIState = UIState.Open;
        }

        public void Close()
        {
            OnClose();
            UIState = UIState.Close;
        }
    }
}
