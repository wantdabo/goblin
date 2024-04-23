using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Lobby
{
    /// <summary>
    /// 大厅界面
    /// </summary>
    public class LobbyView : UIBaseView
    {
        public override UILayer layer =>  UILayer.UIMain;

        protected override string res => "Lobby/LobbyView";
    }
}
