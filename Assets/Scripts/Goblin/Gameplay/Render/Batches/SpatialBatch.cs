using Goblin.Common;
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

        private NativeArray<Vector3> positions;
        private NativeArray<Vector3> tarpositions;
        private NativeArray<Vector3> eulers;
        private NativeArray<Vector3> tareulers;
        private NativeArray<Vector3> tarscales;

        protected override void OnCreate()
        {
            base.OnCreate();
            transforms = new Transform[1000];
            transformaccess = new TransformAccessArray(transforms);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (transformaccess.isCreated) transformaccess.Dispose();
        }

        protected override void OnTick(TickEvent e)
        {
            base.OnTick(e);
            if (false == world.statebucket.SeekStates<SpatialState>(out var states)) return;
            var count = states.Count;
            if (positions.IsCreated) positions.Dispose();
            if (tarpositions.IsCreated) tarpositions.Dispose();
            if (eulers.IsCreated) eulers.Dispose();
            if (tareulers.IsCreated) tareulers.Dispose();
            if (tarscales.IsCreated) tarscales.Dispose();
            
            positions = new NativeArray<Vector3>(count, Allocator.TempJob);
            tarpositions = new NativeArray<Vector3>(count, Allocator.TempJob);
            eulers = new NativeArray<Vector3>(count, Allocator.TempJob);
            tareulers = new NativeArray<Vector3>(count, Allocator.TempJob);
            tarscales = new NativeArray<Vector3>(count, Allocator.TempJob);

            int index = 0;
            foreach (var state in states)
            {
                var ticker = world.statebucket.GetState<TickerState>(state.actor);
                var node = world.GetAgent<NodeAgent>(state.actor);
                transforms[index] = node.go.transform;
                positions[index] = node.go.transform.position;
                tarpositions[index] = state.position;
                eulers[index] = node.go.transform.eulerAngles;
                tareulers[index] = state.euler;
                tarscales[index] = state.scale;
                
                index++;
            }
            
            var job = new SpatialJob
            {
                count = index,
                t = e.tick / GAME_DEFINE.LOGIC_TICK.AsFloat(),
                positions = positions,
                tarpositions = tarpositions,
                eulers = eulers,
                tareulers = tareulers,
                tarscales = tarscales,
            };
            
            var handle = job.Schedule(transformaccess);
            handle.Complete();
        }

        [BurstCompile]
        private struct SpatialJob : IJobParallelForTransform
        {
            public int count;
            public float t;
            [ReadOnly] public NativeArray<Vector3> positions;
            [ReadOnly] public NativeArray<Vector3> tarpositions;
            [ReadOnly] public NativeArray<Vector3> eulers;
            [ReadOnly] public NativeArray<Vector3> tareulers;
            [ReadOnly] public NativeArray<Vector3> tarscales;

            public void Execute(int index, TransformAccess transform)
            {
                if (index >= count) return;
                var position = positions[index];
                var tarposition = tarpositions[index];
                var euler = eulers[index];
                var tareuler = tareulers[index];
                var tarscale = tarscales[index];
                
                transform.position = math.lerp(position, tarposition, t);
                transform.rotation = math.slerp(quaternion.Euler(euler), quaternion.Euler(tareuler), t);
                transform.localScale = tarscale;
            }
        }
    }
}