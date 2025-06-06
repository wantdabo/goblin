using System;
using System.Collections.Generic;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pipeline.Timeline.Layouts.Common
{
    /// <summary>
    /// 管线 Timeline 布局
    /// </summary>
    public class PipelineLayout : ScriptableObject
    {
        /// <summary>
        /// 模型
        /// </summary>
        public int model;
        /// <summary>
        /// 轨道布局映射 PipelineData.Instruct.Index
        /// </summary>
        public List<Dictionary<ushort, List<uint>>> tracks;
        /// <summary>
        /// 序列化数据
        /// </summary>
        [SerializeField]
        private List<Serialize> serializes;

        /// <summary>
        /// 读取序列化数据结构
        /// </summary>
        public void ReadTracks()
        {
            if (null == serializes || serializes.Count <= 0) return;

            tracks = new List<Dictionary<ushort, List<uint>>>(serializes.Count);
            foreach (var serialize in serializes)
            {
                var track = new Dictionary<ushort, List<uint>>(serialize.keys.Count);
                for (int i = 0; i < serialize.keys.Count; i++)
                {
                    track.Add(serialize.keys[i], serialize.values);
                }
                tracks.Add(track);
            }
        }

        /// <summary>
        /// 写入序列化数据结构
        /// </summary>
        public void WriteTracks()
        {
            if (null == tracks || tracks.Count <= 0)
            {
                serializes = null;
                return;
            }

            serializes = new List<Serialize>(tracks.Count);
            foreach (var track in tracks)
            {
                var serialize = new Serialize
                {
                    keys = new List<ushort>(track.Keys),
                    values = new List<uint>()
                };
                foreach (var key in serialize.keys)
                {
                    serialize.values.AddRange(track[key]);
                }
                serializes.Add(serialize);
            }
        }

        /// <summary>
        /// 将管线布局转换为序列化数据
        /// </summary>
        [Serializable]
        private class Serialize
        {
            /// <summary>
            /// 键列表
            /// </summary>
            public List<ushort> keys;
            /// <summary>
            /// 值列表
            /// </summary>
            public List<uint> values;
        }
    }
}