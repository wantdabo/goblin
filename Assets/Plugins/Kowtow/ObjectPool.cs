using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Kowtow.Math;

namespace Kowtow
{
    /// <summary>
    /// 对象池
    /// </summary>
    public static class ObjectPool
    {
        private static readonly ConcurrentDictionary<int, ConcurrentQueue<FPVector3[]>> verticesdict = new();
        private static readonly ConcurrentQueue<List<FPVector3>> verticeslist = new();
        
        public static FPVector3[] GetVertices(int count)
        {
            if (false == verticesdict.TryGetValue(count, out var queue)) 
            {
                queue = new ConcurrentQueue<FPVector3[]>();
                verticesdict.TryAdd(count, queue);
            }
            
            if (false == queue.TryDequeue(out var vertices)) vertices = new FPVector3[count];
            
            return vertices;
        }
        
        public static void SetVertices(FPVector3[] vertices)
        {
            if (false == verticesdict.TryGetValue(vertices.Length, out var queue)) 
            {
                queue = new ConcurrentQueue<FPVector3[]>();
                verticesdict.TryAdd(vertices.Length, queue);
            }
            
            queue.Enqueue(vertices);
        }
        
        public static List<FPVector3> GetVerticesList()
        {
            if (false == verticeslist.TryDequeue(out var vertices)) vertices = new List<FPVector3>();
            
            return vertices;
        }
        
        public static void SetVerticesList(List<FPVector3> vertices)
        {
            vertices.Clear();
            verticeslist.Enqueue(vertices);
        }
    }
}