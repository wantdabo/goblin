using GoblinFramework.Client.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.Client.Gameplay;

namespace GoblinFramework.Client.UI.Login
{
    public class LoginEnterCell : UIBaseCell
    {
        protected override string UIRes => "Login/LoginEnterCell";

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("TryBtn", (eventData) =>
            {
            });
        }
    }
}
