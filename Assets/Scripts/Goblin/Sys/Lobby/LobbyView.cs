using Goblin.Common;
using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Goblin.Sys.Lobby
{
    /// <summary>
    /// 大厅界面
    /// </summary>
    public class LobbyView : UIBaseView
    {
        public override UILayer layer =>  UILayer.UIMain;

        protected override string res => "Lobby/LobbyView";

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }
        
        private void OnTick(TickEvent e) 
        {
            engine.u3dkit.SeekNode<Text>(gameObject, "Ping").text = $"ping: {engine.net.ping} ms";
        }
    }
}
