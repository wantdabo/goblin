﻿using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Login
{
    public class LoginModel : Model<LoginProxy>
    {
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string uuid { get; set; }
        /// <summary>
        /// 登录状态
        /// </summary>
        public bool signined { get; set; }
    }
}
