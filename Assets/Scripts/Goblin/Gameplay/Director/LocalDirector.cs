using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Goblin.Common;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Goblin.RendererFeatures;
using UnityEngine;

namespace Goblin.Gameplay.Director
{
    /// <summary>
    /// 本地单机, 本地导演
    /// </summary>
    public class LocalDirector : GameplayDirector
    {
        /// <summary>
        /// 是否渲染 (驱动 World)
        /// </summary>
        public override bool rendering
        {
            get
            {
                if (restoreing) return false;
                if (null == stage || StageState.Ticking != stage.state) return false;
                
                return true; 
            }
        }

        /// <summary>
        /// 时间缩放
        /// </summary>
        public float timescale => stage.timescale.AsFloat();
        /// <summary>
        /// 逻辑场景
        /// </summary>
        private Stage stage { get; set; }
        /// <summary>
        /// 是否正在恢复
        /// </summary>
        private bool restoreing { get; set; } = false;
        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object @lock = new();
        /// <summary>
        /// RIL 队列
        /// </summary>
        private readonly Queue<IRIL> rilqueue = new();
        /// <summary>
        /// RIL_DIFF 队列
        /// </summary>
        private readonly Queue<IRIL_DIFF> diffqueue = new();
        /// <summary>
        /// RIL 事件队列
        /// </summary>
        private readonly Queue<IRIL_EVENT> eventqueue = new();
        /// <summary>
        /// 碰撞盒列表
        /// </summary>
        private readonly List<ColliderInfo> colliders = new();

        protected override void OnCreateGame()
        {
            // 预载所有 Pipeline
            foreach (var pipelinecfg in engine.cfg.location.PipelineInfos.DataList) PipelineDataReader.PreloadPipelineData((uint)pipelinecfg.Id, engine.gameres.location.LoadPipelineSync(pipelinecfg.Id.ToString()));
            // 初始化逻辑层
            stage = new Stage().Initialize(data.sdata);
            // 监听 RIL 渲染状态
            stage.onril += OnRIL;
            // 监听 RIL_DIFF 渲染状态
            stage.ondiff += OnDiff;
            // 监听 RIL 事件
            stage.onevent += OnEvent;
        }

        protected override void OnDestroyGame()
        {
            // 销毁逻辑层
            stage.Dispose();
            // 取消监听 RIL 渲染状态
            stage.onril -= OnRIL;
            // 取消监听 RIL_DIFF 渲染状态
            stage.ondiff -= OnDiff;
            // 取消监听 RIL 事件
            stage.onevent -= OnEvent;
        }

        protected override void OnStartGame() => stage.Start();
        protected override void OnPauseGame() => stage.Pause();
        protected override void OnResumeGame() => stage.Resume();
        protected override void OnStopGame() => stage.Stop();

        protected override void OnSnapshot()
        {
            world.Snapshot();
            stage.Snapshot();
        }

        protected override void OnRestore()
        {
            restoreing = true;
            world.rilbucket.LossAllRIL();
            lock (@lock)
            {
                while (rilqueue.TryDequeue(out var ril))
                {
                    ril.Reset();
                    RILCache.Set(ril);
                }

                while (diffqueue.TryDequeue(out var diff))
                {
                    diff.Reset();
                    RILCache.Set(diff);
                }

                while (eventqueue.TryDequeue(out var e))
                {
                    e.Reset();
                    RILCache.Set(e);
                }
            }
            
            world.Restore();
        }

        protected override void OnTick()
        {
            if (restoreing) return;
            
            DrawPhys();
            lock (@lock)
            {
                while(rilqueue.TryDequeue(out var ril)) world.rilbucket.SetRIL(ril);
                while (diffqueue.TryDequeue(out var diff)) world.rilbucket.SetDiff(diff);
                while (eventqueue.TryDequeue(out var e)) world.rilbucket.SetEvent(e);
            }
        }
        
        /// <summary>
        /// 绘制物理
        /// </summary>
        private void DrawPhys()
        {
            if (false == engine.proxy.gameplay.physdraw) return;

            lock (stage.detection.@lock)
            {
                Color color = Color.yellow;

                foreach (var raycast in stage.detection.raycasts)
                {
                    DrawPhysRendererFeature.DrawPhysPass.DrawRay(raycast.center.ToVector3(), raycast.dire.ToVector3(), raycast.dis.AsFloat(), color);
                }
                
                foreach (var overlapbox in stage.detection.overlapboxes)
                {
                    DrawPhysRendererFeature.DrawPhysPass.DrawCube(overlapbox.position.ToVector3(), overlapbox.rotation.ToQuaternion(), overlapbox.size.ToVector3(), color);
                }
                
                foreach (var overlapsphere in stage.detection.overlapspheres)
                {
                    DrawPhysRendererFeature.DrawPhysPass.DrawSphere(overlapsphere.position.ToVector3(), overlapsphere.radius.AsFloat(), color);
                }
            }

            lock (@lock)
            {
                foreach (var collider in colliders)
                {
                    if (false == stage.SeekBehaviorInfo(collider.actor, out SpatialInfo spatial)) continue;
                    Color color = new Color(210 / 255f, 255 / 255f, 0 / 255f);
                    switch (collider.shape)
                    {
                        case COLLISION_DEFINE.COLLIDER_BOX:
                            var center = (spatial.position + collider.box.offset).ToVector3();
                            var rotation = Quaternion.Euler(spatial.euler.ToVector3());
                            var size = collider.box.size.ToVector3();
                            DrawPhysRendererFeature.DrawPhysPass.DrawCube(center, rotation, size, color);
                            break;
                        case COLLISION_DEFINE.COLLIDER_SPHERE:
                            center = (spatial.position + collider.sphere.offset).ToVector3();
                            var radius = collider.sphere.radius.AsFloat();
                            DrawPhysRendererFeature.DrawPhysPass.DrawSphere(center, radius, color);
                            break;
                    }
                }
            }
        }

        protected override void OnStep()
        {
            // 如果正在恢复, 则直接恢复
            if (restoreing)
            {
                stage.Restore();
                restoreing = false;

                return;
            }

            if (null == stage || StageState.Ticking != stage.state) return;

            while (world.input.TryDequeueCommand(out var command))
            {
                stage.SetCommand(command);
                command.Reset();
                ObjectPool.Set(command);
            }

            var joystick = world.input.GetInput(INPUT_DEFINE.JOYSTICK);
            var ba = world.input.GetInput(INPUT_DEFINE.BA);
            stage.SetInput(world.selfseat, INPUT_DEFINE.JOYSTICK, joystick.press, joystick.dire);
            stage.SetInput(world.selfseat, INPUT_DEFINE.BA, ba.press, ba.dire);
            stage.Step();

            lock (@lock)
            {
                colliders.Clear();
                if (false == stage.SeekBehaviorInfos(out List<ColliderInfo> infos)) return;
                colliders.AddRange(infos);
            }
        }

        /// <summary>
        /// 处理 RIL 渲染状态
        /// </summary>
        /// <param name="ril">RIL 渲染状态</param>
        private void OnRIL(IRIL ril)
        {
            lock (@lock)
            {
                rilqueue.Enqueue(ril);
            }
        }

        /// <summary>
        /// 处理 RIL_DIFF 渲染状态
        /// </summary>
        /// <param name="diff">差异状态</param>
        private void OnDiff(IRIL_DIFF diff)
        {
            lock (@lock)
            {
                diffqueue.Enqueue(diff);
            }
        }

        /// <summary>
        /// 处理 RIL 事件
        /// </summary>
        /// <param name="e">RIL 事件</param>
        private void OnEvent(IRIL_EVENT e)
        {
            lock (@lock)
            {
                eventqueue.Enqueue(e);
            }
        }
    }
}