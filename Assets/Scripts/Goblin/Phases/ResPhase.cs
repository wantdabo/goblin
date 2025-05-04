using Goblin.Common.FSM;
using Goblin.Sys.Common;
using Goblin.Sys.Initialize.View;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YooAsset;

namespace Goblin.Phases
{
    /// <summary>
    /// 资源初始化阶段
    /// </summary>
    public class ResPhase : State
    {
        protected override List<Type> passes => new() { typeof(HotfixPhase) };

        /// <summary>
        /// 就绪
        /// </summary>
        public bool finished { get; private set; }

        public override bool OnValid()
        {
            return false == finished;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            finished = true;
        }
    }
}
