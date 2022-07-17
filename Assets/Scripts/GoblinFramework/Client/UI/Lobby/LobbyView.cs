using GoblinFramework.Client.UI.Base;
using GoblinFramework.Client.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoblinFramework.Client.UI.Lobby
{
    public class LobbyView : UIBaseView
    {
        protected override string UIRes => "Lobby/LobbyView";

        public override GameUI.UILayer UILayer => GameUI.UILayer.UIMain;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            Engine.U3D.SeekNode<Text>(go, "Title").text = "GoblinFramework";

            int counter = 0;
            var clock = AddComp<Client.Common.ClockComp>();
            clock.Start(() =>
            {
                counter++;
                Engine.U3D.SeekNode<Text>(go, "ClockText").text = counter.ToString();
            }, 1, -1);
        }
    }
}
