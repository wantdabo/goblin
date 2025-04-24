using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.States;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Goblin.Gameplay.Render.Batches
{
    /// <summary>
    /// 空间批处理
    /// </summary>
    public class SpatialBatch : Batch
    {
        /// <summary>
        /// Jobs Transform 数组
        /// </summary>
        private Transform[] transforms;
        /// <summary>
        /// Jobs Transform 访问数组
        /// </summary>
        private TransformAccessArray transformaccess;
        /// <summary>
        /// Jobs 位置数组
        /// </summary>
        private NativeArray<float3> positions;
        /// <summary>
        /// Jobs 目标位置数组
        /// </summary>
        private NativeArray<float3> tarpositions;
        /// <summary>
        /// Jobs 旋转数组
        /// </summary>
        private NativeArray<quaternion> rotations;
        /// <summary>
        /// Jobs 目标旋转数组
        /// </summary>
        private NativeArray<quaternion> tarrotations;
        /// <summary>
        /// Jobs 目标缩放数组
        /// </summary>
        private NativeArray<float3> tarscales;

        protected override void OnCreate()
        {
            base.OnCreate();
            // 初始化 Jobs 相关数据, Transform 数组大小为 1000 (如果超过，将会分批次，上限 1000)
            transforms = new Transform[1000];
            transformaccess = new TransformAccessArray(transforms);
            positions = new NativeArray<float3>(transforms.Length, Allocator.TempJob);
            tarpositions = new NativeArray<float3>(transforms.Length, Allocator.TempJob);
            rotations = new NativeArray<quaternion>(transforms.Length, Allocator.TempJob);
            tarrotations = new NativeArray<quaternion>(transforms.Length, Allocator.TempJob);
            tarscales = new NativeArray<float3>(transforms.Length, Allocator.TempJob);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 释放 Jobs 相关数据
            transforms = null;
            if (positions.IsCreated) positions.Dispose();
            if (tarpositions.IsCreated) tarpositions.Dispose();
            if (rotations.IsCreated) rotations.Dispose();
            if (tarrotations.IsCreated) tarrotations.Dispose();
            if (tarscales.IsCreated) tarscales.Dispose();
            if (transformaccess.isCreated) transformaccess.Dispose();
        }

        protected override void OnTick(TickEvent e)
        {
            base.OnTick(e);
            if (false == world.statebucket.SeekStates<SpatialState>(out var states)) return;
            // 收集所有需要更新的节点, 进行 Jobs 处理
            int index = 0;
            int statecnt = states.Count;
            foreach (var state in states)
            {
                var node = world.GetAgent<NodeAgent>(state.actor);
                if (null == node || ChaseStatus.Arrived == node.status) statecnt--;
                else
                {
                    // 收集
                    transforms[index] = node.go.transform;
                    positions[index] = node.go.transform.position;
                    tarpositions[index] = state.position;
                    rotations[index] = Quaternion.Euler(node.go.transform.eulerAngles);
                    tarrotations[index] = Quaternion.Euler(state.euler);
                    tarscales[index] = state.scale;
                    index++;
                }
                
                // 如果超过了 Jobs 的最大数量, 则分批次处理
                if (transforms.Length == index || statecnt == index)
                {
                    var job = new SpatialJob
                    {
                        count = index,
                        t = Mathf.Clamp01(e.tick / GAME_DEFINE.LOGIC_TICK.AsFloat()),
                        positions = positions,
                        tarpositions = tarpositions,
                        rotations = rotations,
                        tarrotations = tarrotations,
                        tarscales = tarscales,
                    };
                    index = 0;
                    
                    // Jobs.Execute
                    transformaccess.SetTransforms(transforms);
                    var handle = job.Schedule(transformaccess);
                    handle.Complete();
                }
            }
            states.Clear();
            ObjectCache.Set(states);
        }

        /// <summary>
        /// 空间批处理 Jobs
        /// </summary>
        [BurstCompile]
        private struct SpatialJob : IJobParallelForTransform
        {
            /// <summary>
            /// 批处理数量 (下面的数组数据, 并非全部可以装满, 因此需要一个边界)
            /// </summary>
            public int count;
            /// <summary>
            /// 插值参数 (0 - 1)
            /// </summary>
            public float t;
            /// <summary>
            /// Jobs 位置数组
            /// </summary>
            [ReadOnly] public NativeArray<float3> positions;
            /// <summary>
            /// Jobs 目标位置数组
            /// </summary>
            [ReadOnly] public NativeArray<float3> tarpositions;
            /// <summary>
            /// Jobs 旋转数组
            /// </summary>
            [ReadOnly] public NativeArray<quaternion> rotations;
            /// <summary>
            /// Jobs 目标旋转数组
            /// </summary>
            [ReadOnly] public NativeArray<quaternion> tarrotations;
            /// <summary>
            /// Jobs 目标缩放数组
            /// </summary>
            [ReadOnly] public NativeArray<float3> tarscales;

            public void Execute(int index, TransformAccess transform)
            {
                if (index >= count) return;
                var position = positions[index];
                var tarposition = tarpositions[index];
                var rotation = rotations[index];
                var tarrotation = tarrotations[index];
                var tarscale = tarscales[index];
                
                // 位置插值
                transform.position = math.lerp(position, tarposition, t);
                // 旋转插值
                transform.rotation = math.slerp(rotation, tarrotation, t);
                // 缩放赋值
                transform.localScale = tarscale;
            }
        }
    }
}