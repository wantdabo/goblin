using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.UI
{
    public class LobbyView : UIBaseView
    {
        protected override string UIRes => "Lobby/LobbyView";

        public override GameUI.UILayer UILayer => GameUI.UILayer.UIMain;

        protected override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
