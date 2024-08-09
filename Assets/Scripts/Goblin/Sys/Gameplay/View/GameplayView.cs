using Goblin.Common;
using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Gameplay.View
{
    /// <summary>
    /// Gameplay
    /// </summary>
    public class GameplayView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;

        protected override string res => "Gameplay/GameplayView";

        private Text descText;

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            descText = engine.u3dkit.SeekNode<Text>(gameObject, "Desc");
        }

        private void OnFixedTick(FixedTickEvent e)
        {
            var x = 0;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) x = -1;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) x = 1;
            engine.proxy.gameplay.stage.SetInput(x);

            if (Input.GetKey(KeyCode.Q))
            {
                engine.proxy.lobby.C2SDestroyRoom();
            }

            descText.text = $"<color=#C3F002>PRESS KEY 'Q' TO EXIT</color>\n\n" +
                $"GAMEPLAY INFO\n" +
                $"PING : {engine.proxy.gameplay.stage.net.ping} MS\n" +
                $"FRAME : {engine.proxy.gameplay.stage.frame} / {engine.proxy.gameplay.stage.mframe}";
        }
    }
}
