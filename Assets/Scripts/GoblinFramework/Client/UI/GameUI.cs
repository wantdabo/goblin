using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.UI
{
    public class GameUI : ClientComp
    {
        public readonly string UIMainLayer = "UIMainLayer";
        public readonly string UIAlertLayer = "UIAlertLayer";
        public readonly string UITopLayer = "UITopLayer";

        internal enum UIState
        {
            Free,
            Loading,
            Open,
            Close
        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
