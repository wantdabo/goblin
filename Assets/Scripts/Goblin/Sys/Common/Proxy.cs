using Goblin.Common;
using Goblin.Core;
using Goblin.Sys.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Common
{
    /// <summary>
    /// 数据保存事件
    /// </summary>
    public struct DBSaveEvent : IEvent { }

    /// <summary>
    /// 系统层，代理
    /// </summary>
    public class Proxy : Comp
    {
        public Eventor eventor;

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            eventor = null;
        }
    }
}
