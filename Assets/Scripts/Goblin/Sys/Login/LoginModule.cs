using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Login
{
    public class LoginModule : Module<LoginProxy>
    {
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 登录状态
        /// </summary>
        public bool signined { get; set; }
    }
}
