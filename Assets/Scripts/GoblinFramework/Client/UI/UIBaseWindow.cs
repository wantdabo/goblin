using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GoblinFramework.Client.UI.GameUI;

namespace GoblinFramework.Client.UI
{
    public abstract class UIBaseWindow : UIBase
    {
        protected abstract string UILayer { get; }

        internal UIState uiState = UIState.Free;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiState = UIState.Free;
        }

        public void Open()
        {
            //if (null == gameObject)
            //{
            //    Engine.GameRes.LoadAssetSync<GameObject>(UIName, (obj) =>
            //    {
            //        gameObject = GameObject.Instantiate(obj);
            //        OnBuildUI();
            //        OnBindEvent();
            //        OnOpen();
            //    });
            //    uiState = UIState.Loading;

            //    return;
            //}

            OnOpen();
            uiState = UIState.Open;
        }

        public void Close()
        {
            OnClose();
            uiState = UIState.Close;
        }
    }
}
