using GoblinFramework.Client.Common;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Gameplay.Resolves
{
    public class SyncResolver<CT> : CComp
    {
        /// <summary>
        /// 指令解析方法
        /// </summary>
        /// <typeparam name="T">指令类型</typeparam>
        /// <param name="cmd">指令</param>
        public virtual void Resolve<T>(T cmd) where T : CT
        {

        }
    }
}
