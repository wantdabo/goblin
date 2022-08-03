using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.RIL.RILS
{
    public class RILModel : RIL
    {
        /// <summary>
        /// 以后会是配置表的 ID 来的
        /// </summary>
        public string modelName;

        public override RILType Type => RILType.RILModel;
    }

    //public class RILDress : RIL
    //{
    //    /// <summary>
    //    /// 部位 ID，头、手、腿装扮
    //    /// </summary>
    //    public int part;
    //    /// <summary>
    //    /// 以后会是配置表的 ID 来的
    //    /// </summary>
    //    public string modelName;

    //    public override RILType Type => throw new NotImplementedException();
    //}
}
