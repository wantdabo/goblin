using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.UI.Base
{
    public abstract class UIBaseCell : UIBase
    {
        private GameObject container;

        public GameObject Container { get { return container; } set { container = value; } }
    }
}
