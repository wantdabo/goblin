using Goblin.Core;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    public class Config : Behavior
    {
        /// <summary>
        /// 配置表定位器
        /// </summary>
        public Tables location { get; private set; }
        /// <summary>
        /// FP 转整型的乘法系数（1000 表示 1）
        /// </summary>
        public int fp2int { get; private set; } = 1000;
        /// <summary>
        /// 整型转 FP 的乘法系数（1000 表示 1）
        /// </summary>
        public FP int2fp { get; private set; } = FP.EN3;
        
        protected override void OnAssemble()
        {
            base.OnAssemble();
            // TODO 后续加宏, 判断非 UNITY 环境使用 File.ReadAllBytes 构建
            location = Export.engine.cfg.location;
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            location = null;
        }
    }
}