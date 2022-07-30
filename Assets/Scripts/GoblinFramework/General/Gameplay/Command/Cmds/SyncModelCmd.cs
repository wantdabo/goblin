using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.Command.Cmds
{
    public class SyncModelCmd : SyncCmd
    {
        /// <summary>
        /// 以后会是配置表的 ID 来的
        /// </summary>
        public string modelName;

        public override CType Type => CType.SyncModelCmd;
    }

    //public class SyncDressCmd : SyncCmd 
    //{
    //    /// <summary>
    //    /// 部位 ID，头、手、腿装扮
    //    /// </summary>
    //    public int part;
    //    /// <summary>
    //    /// 以后会是配置表的 ID 来的
    //    /// </summary>
    //    public string modelName;
    //}
}
