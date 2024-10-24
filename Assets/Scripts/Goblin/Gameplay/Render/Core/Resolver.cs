using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// 渲染指令解释事件
    /// </summary>
    public struct RILResolveEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 渲染指令
        /// </summary>
        public IRIL ril { get; set; }
    }
    
    /// <summary>
    /// 渲染指令解释器
    /// </summary>
    public abstract class Resolver : Comp
    {
        /// <summary>
        /// 渲染指令 ID
        /// </summary>
        public abstract ushort id { get; }
        /// <summary>
        /// Actor
        /// </summary>
        public Actor actor { get; set; }
        
        /// <summary>
        /// 渲染指令初始化
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="ril">渲染指令</param>
        public void Awake(uint frame, IRIL ril)
        {
            OnAwake(frame, ril);
        }

        /// <summary>
        /// 渲染指令解释
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="ril">渲染指令</param>
        public void Resolve(uint frame, IRIL ril)
        {
            OnResolve(frame, ril);
        }
        
        /// <summary>
        /// 渲染指令初始化
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="ril">渲染指令</param>
        protected abstract void OnAwake(uint frame, IRIL ril);
        /// <summary>
        /// 渲染指令解释
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="ril">渲染指令</param>
        protected abstract void OnResolve(uint frame, IRIL ril);
    }
    
    /// <summary>
    /// 渲染指令解释器
    /// </summary>
    /// <typeparam name="T">渲染指令类型</typeparam>
    public abstract class Resolver<T> : Resolver where T : IRIL
    {
        protected override void OnAwake(uint frame, IRIL ril)
        {
            OnAwake(frame, (T)ril);
        }

        protected override void OnResolve(uint frame, IRIL ril)
        {
            OnResolve(frame, (T)ril);
        }
        
        /// <summary>
        /// 渲染指令初始化
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="ril">渲染指令</param>
        protected abstract void OnAwake(uint frame, T ril);
        /// <summary>
        /// 渲染指令解释
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="ril">渲染指令</param>
        protected abstract void OnResolve(uint frame, T ril);
    }
}
