using System;
using System.Collections.Generic;
using BEPUutilities.DataStructures;
using BEPUutilities.ResourceManagement;
using FixMath.NET;

namespace BEPUutilities
{
    public static partial class ConvexHullHelper
    {
        /// <summary>
        /// Represents a cell in space which is already occupied by a point.  Any other points which resolve to the same cell are considered redundant.
        /// </summary>
        public struct BlockedCell : IEquatable<BlockedCell>
        {
            public int X;
            public int Y;
            public int Z;

            public override int GetHashCode()
            {
                const long p1 = 961748927L;
                const long p2 = 961748941L;
                const long p3 = 982451653L;
                return (int)(X * p1 + Y * p2 + Z * p3);
            }

            public override bool Equals(object obj)
            {
                return this.Equals((BlockedCell)obj);
            }

            public bool Equals(BlockedCell other)
            {
                return other.X == X && other.Y == Y && other.Z == Z;
            }
        }

		/// <summary>
		/// Contains and manufactures cell sets used by the redundant point remover.  To minimize memory usage, this can be cleared
		/// after using the RemoveRedundantPoints if it isn't going to be used again.
		/// </summary>
		public static LockingResourcePool<HashSet<BlockedCell>> BlockedCellSets = new LockingResourcePool<HashSet<BlockedCell>>();

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell of size 0.001.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        public static void RemoveRedundantPoints(IList<Vector3> points)
        {
            RemoveRedundantPoints(points, F64.C0p001);
        }

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        /// <param name="cellSize">Size of cells to determine redundancy.</param>
        public static void RemoveRedundantPoints(IList<Vector3> points, Fix64 cellSize)
        {
            var rawPoints = CommonResources.GetVectorList();
            rawPoints.AddRange(points);
            RemoveRedundantPoints(rawPoints, cellSize);
            points.Clear();
            for (int i = 0; i < rawPoints.Count; ++i)
            {
                points.Add(rawPoints.Elements[i]);
            }
            CommonResources.GiveBack(rawPoints);
        }

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell of size 0.001.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        public static void RemoveRedundantPoints(RawList<Vector3> points)
        {
            RemoveRedundantPoints(points, F64.C0p001);
        }

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        /// <param name="cellSize">Size of cells to determine redundancy.</param>
        public static void RemoveRedundantPoints(RawList<Vector3> points, Fix64 cellSize)
        {
            var set = BlockedCellSets.Take();
            for (int i = points.Count - 1; i >= 0; --i)
            {
                var element = points.Elements[i];
                var cell = new BlockedCell
                {
                    X = (int)Fix64.Floor(element.X / cellSize),
                    Y = (int)Fix64.Floor(element.Y / cellSize),
                    Z = (int)Fix64.Floor(element.Z / cellSize) 
                };
                if (set.Contains(cell))
                {
                    points.FastRemoveAt(i);
                }
                else
                {
                    set.Add(cell);
                    //TODO: Consider adding adjacent cells to guarantee that a point on the border between two cells will still detect the presence
                    //of a point on the opposite side of that border.
                }
            }
            set.Clear();
            BlockedCellSets.GiveBack(set);
        }

    }
}
