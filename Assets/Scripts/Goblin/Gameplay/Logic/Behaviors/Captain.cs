using System.Collections.Generic;
using Goblin.Gameplay.Logic.Commands.Common;
using Goblin.Gameplay.Logic.Commands.Soliders;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 输入指令
    /// </summary>
    public class Captain : Behavior
    {
        /// <summary>
        /// 输入指令队列
        /// </summary>
        private Queue<Command> cmdqueue { get; set; }
        /// <summary>
        /// 输入指令执行器列表
        /// </summary>
        private Dictionary<ushort, Solider> soliderdict { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();

            cmdqueue = ObjectCache.Ensure<Queue<Command>>();
            
            // 注册输入指令执行器
            soliderdict = ObjectCache.Ensure<Dictionary<ushort, Solider>>();
            void Solider<T>(ushort id) where T : Solider, new()
            {
                var solider = ObjectCache.Ensure<T>();
                soliderdict.Add(id, solider.Load(stage));
            }
            Solider<GMSolider>(INPUT_DEFINE.GM);
            Solider<TimeScaleSolider>(INPUT_DEFINE.TIMESCALE);
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            
            while (cmdqueue.TryDequeue(out var command))
            {
                command.Reset();
                ObjectCache.Set(command);
            }
            ObjectCache.Set(cmdqueue);
            
            // 卸载输入指令执行器
            foreach (var solider in soliderdict.Values)
            {
                solider.Unload();
                ObjectCache.Set(solider);
            }
            soliderdict.Clear();
            ObjectCache.Set(soliderdict);
        }
        
        public void SetCommand(Command command)
        {
            if (null == command) return;
            cmdqueue.Enqueue(command);
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            // 执行输入指令
            while (cmdqueue.TryDequeue(out var command))
            {
                if (soliderdict.TryGetValue(command.id, out var solider)) solider.Execute(command);
                
                command.Reset();
                ObjectCache.Set(command);
            }
        }
    }
}