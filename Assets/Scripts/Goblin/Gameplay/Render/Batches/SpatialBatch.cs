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
    public class SpatialBatch : Batch
    {
        private Transform[] transforms;
        private TransformAccessArray transformaccess;
        private NativeArray<float3> positions;
        private NativeArray<float3> tarpositions;
        private NativeArray<quaternion> rotations;
        private NativeArray<quaternion> tarrotations;
        private NativeArray<float3> tarscales;

        protected override void OnCreate()
        {
            base.OnCreate();
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
            int index = 0;
            int statecnt = states.Count;
            foreach (var state in states)
            {
                var node = world.GetAgent<NodeAgent>(state.actor);
                
                if (null == node || ChaseStatus.Arrived == node.status) statecnt--;
                else
                {
                    transforms[index] = node.go.transform;
                    positions[index] = node.go.transform.position;
                    tarpositions[index] = state.position;
                    rotations[index] = Quaternion.Euler(node.go.transform.eulerAngles);
                    tarrotations[index] = Quaternion.Euler(state.euler);
                    tarscales[index] = state.scale;
                    index++;
                }
                
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
                    
                    transformaccess.SetTransforms(transforms);
                    var handle = job.Schedule(transformaccess);
                    handle.Complete();
                }
            }
            states.Clear();
            ObjectCache.Set(states);
        }

        private void Fire()
        {
            
        }

        [BurstCompile]
        private struct SpatialJob : IJobParallelForTransform
        {
            public int count;
            public float t;
            [ReadOnly] public NativeArray<float3> positions;
            [ReadOnly] public NativeArray<float3> tarpositions;
            [ReadOnly] public NativeArray<quaternion> rotations;
            [ReadOnly] public NativeArray<quaternion> tarrotations;
            [ReadOnly] public NativeArray<float3> tarscales;

            public void Execute(int index, TransformAccess transform)
            {
                if (index >= count) return;
                var position = positions[index];
                var tarposition = tarpositions[index];
                var rotation = rotations[index];
                var tarrotation = tarrotations[index];
                var tarscale = tarscales[index];
                
                transform.position = math.lerp(position, tarposition, t);
                transform.rotation = math.slerp(rotation, tarrotation, t);
                transform.localScale = tarscale;
            }
        }
    }
}