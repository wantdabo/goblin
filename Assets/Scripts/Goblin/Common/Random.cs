﻿using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Common
{
    /// <summary>
    /// 随机器
    /// </summary>
    public class Random : Comp
    {
        private System.Random random;

        /// <summary>
        /// 随机种子
        /// </summary>
        public int seed { get; private set; } = -1;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="s">种子</param>
        public void Initial(int s)
        {
            seed = s;
            random = new System.Random(seed);
        }

        /// <summary>
        /// 整数范围随机
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>结果</returns>
        public int Range(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// 浮点数范围随机
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>结果</returns>
        public float Range(float min, float max)
        {
            return random.Next((int)(min * engine.cfg.float2Int), (int)(max * engine.cfg.float2Int)) * engine.cfg.int2Float;
        }

        /// <summary>
        /// 扇形上随机点
        /// </summary>
        /// <param name="minDeg">最小角度</param>
        /// <param name="maxDeg">最大角度</param>
        /// <param name="radius">半径</param>
        /// <returns>坐标</returns>
        public Vector2 RandSectorPoint(float minDeg, float maxDeg, float radius = 1f)
        {
            return SectorPoint(Range(minDeg, maxDeg), radius);
        }

        /// <summary>
        /// 圆上随机坐标
        /// </summary>
        /// <param name="deg">角度</param>
        /// <param name="radius">半径</param>
        /// <returns>对应扇形上坐标</returns>
        public Vector2 SectorPoint(float deg, float radius = 1f)
        {
            return new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad)).normalized * radius;
        }

        /// <summary>
        /// 圆内随机坐标
        /// </summary>
        /// <param name="radius">半径</param>
        /// <returns>圆内某一点随机</returns>
        public Vector2 RandCirclePoint(float radius = 1)
        {
            return RandDire() * radius;
        }

        /// <summary>
        /// 随机一个方向
        /// </summary>
        /// <returns></returns>
        public Vector2 RandDire()
        {
            return new Vector2(Range(-100, 100), Range(-100, 100)).normalized;
        }

        /// <summary>
        /// 打乱容器内元素
        /// </summary>
        /// <param name="arr">所有元素数组</param>
        /// <returns>乱序元素数组</returns>
        public T[] RandomElement<T>(T[] arr) where T : struct
        {
            // 打乱数组元素
            for (int i = 0; i < arr.Length; i++)
            {
                int index = Range(0, arr.Length);
                T temp = arr[i];
                arr[i] = arr[index];
                arr[index] = temp;
            }

            return arr;
        }

        /// <summary>
        /// 打乱容器内元素
        /// </summary>
        /// <param name="list">所有元素数组</param>
        /// <returns>乱序元素数组</returns>
        public List<T> RandomElement<T>(List<T> list) where T : class
        {
            // 打乱数组元素
            for (int i = 0; i < list.Count; i++)
            {
                int index = Range(0, list.Count);
                T temp = list[i];
                list[i] = list[index];
                list[index] = temp;
            }

            return list;
        }

        /// <summary>
        /// 抖动算法
        /// </summary>
        /// <param name="originPos">初始位置</param>
        /// <param name="intensity">抖动强度</param>
        /// <param name="t">步长</param>
        /// <returns>坐标</returns>
        public Vector2 Shake(Vector2 originPos, float intensity, float t)
        {
            var pos = originPos;
            var randPos = pos + RandCirclePoint() * intensity;
            pos = Vector2.Lerp(originPos, randPos, t);

            return pos;
        }
    }
}