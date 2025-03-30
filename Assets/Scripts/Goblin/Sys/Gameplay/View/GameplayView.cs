using Goblin.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Gameplay.View
{
    public class GameplayView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;
        protected override string res => "Gameplay/GameplayView";
    }
}
