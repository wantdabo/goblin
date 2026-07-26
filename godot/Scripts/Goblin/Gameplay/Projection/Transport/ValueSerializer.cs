using System;
using System.Collections.Generic;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Projection.Transport;

/// <summary>
/// 值类型编码常量
/// </summary>
internal static class ValueTypeCode
{
    public const byte NULL = 0;
    public const byte INT32 = 1;
    public const byte UINT32 = 2;
    public const byte BYTE = 3;
    public const byte FP = 4;
    public const byte FPVECTOR3 = 5;
    public const byte FPQUATERNION = 6;
    public const byte INT64 = 7;
    public const byte UINT64 = 8;
}

/// <summary>
/// 可序列化的单值包装（MessagePack 安全）
/// </summary>
[MessagePackObject]
public class SerializedValue
{
    /// <summary>
    /// 值类型代码（见 ValueTypeCode）
    /// </summary>
    [Key(0)]
    public byte code { get; set; }

    /// <summary>
    /// 原始数据（long 数组编码，null 表示空值）
    /// </summary>
    [Key(1)]
    public long[] data { get; set; }
}

/// <summary>
/// 值序列化器 — FP/FPVector3/FPQuaternion 与可序列化形式的互转
/// Bug 19 修复：避免自定义结构体在 MessagePack object[] 中静默丢失
/// Phase 2 网络模式时 SG 将为每个字段类型生成专用序列化器替代本转换器
/// </summary>
public static class ValueSerializer
{
    /// <summary>
    /// object[] → SerializedValue[]（发送端转换）
    /// </summary>
    public static List<SerializedValue> SerializeValues(object[] values)
    {
        if (null == values) return null;

        var result = new List<SerializedValue>(values.Length);
        foreach (var val in values)
        {
            result.Add(SerializeOne(val));
        }
        return result;
    }

    /// <summary>
    /// SerializedValue[] → object[]（接收端转换）
    /// </summary>
    public static object[] DeserializeValues(List<SerializedValue> serialized)
    {
        if (null == serialized) return null;

        var result = new object[serialized.Count];
        for (var i = 0; i < serialized.Count; i++)
        {
            result[i] = DeserializeOne(serialized[i]);
        }
        return result;
    }

    /// <summary>
    /// 单值序列化
    /// </summary>
    private static SerializedValue SerializeOne(object value)
    {
        if (null == value) return new SerializedValue { code = ValueTypeCode.NULL };

        switch (value)
        {
            case int i:
                return new SerializedValue { code = ValueTypeCode.INT32, data = new long[] { (long)i } };
            case uint ui:
                return new SerializedValue { code = ValueTypeCode.UINT32, data = new long[] { (long)ui } };
            case byte b:
                return new SerializedValue { code = ValueTypeCode.BYTE, data = new long[] { (long)b } };
            case FP fp:
                return new SerializedValue { code = ValueTypeCode.FP, data = new long[] { (long)fp.RawValue } };
            case FPVector3 v3:
                return new SerializedValue
                {
                    code = ValueTypeCode.FPVECTOR3,
                    data = new long[] { (long)v3.x.RawValue, (long)v3.y.RawValue, (long)v3.z.RawValue },
                };
            case FPQuaternion q:
                return new SerializedValue
                {
                    code = ValueTypeCode.FPQUATERNION,
                    data = new long[] { (long)q.x.RawValue, (long)q.y.RawValue, (long)q.z.RawValue, (long)q.w.RawValue },
                };
            case long l:
                return new SerializedValue { code = ValueTypeCode.INT64, data = new long[] { l } };
            case ulong ul:
                return new SerializedValue { code = ValueTypeCode.UINT64, data = new long[] { (long)(ul >> 32), (long)(ul & 0xFFFFFFFF) } };
            default:
                // 未识别的引用类型（GBLDict/GBLList 等）— Phase 2 由 SG 专用序列化器处理
                // 当前网络模式下静默丢弃，避免 MessagePack 序列化 object[] 失败
                System.Diagnostics.Debug.WriteLine(
                    $"ValueSerializer: 不支持的类型 '{value.GetType().FullName}'，值被丢弃。Phase 2 请使用 SG 专用序列化器。");
                return new SerializedValue { code = ValueTypeCode.NULL };
        }
    }

    /// <summary>
    /// 单值反序列化
    /// </summary>
    private static object DeserializeOne(SerializedValue sv)
    {
        if (null == sv || ValueTypeCode.NULL == sv.code) return null;

        switch (sv.code)
        {
            case ValueTypeCode.INT32:
                return (int)sv.data[0];
            case ValueTypeCode.UINT32:
                return (uint)sv.data[0];
            case ValueTypeCode.BYTE:
                return (byte)sv.data[0];
            case ValueTypeCode.FP:
                return FP.FromRaw(sv.data[0]);
            case ValueTypeCode.FPVECTOR3:
                return new FPVector3(
                    FP.FromRaw(sv.data[0]),
                    FP.FromRaw(sv.data[1]),
                    FP.FromRaw(sv.data[2]));
            case ValueTypeCode.FPQUATERNION:
                return new FPQuaternion(
                    FP.FromRaw(sv.data[0]),
                    FP.FromRaw(sv.data[1]),
                    FP.FromRaw(sv.data[2]),
                    FP.FromRaw(sv.data[3]));
            case ValueTypeCode.INT64:
                return sv.data[0];
            case ValueTypeCode.UINT64:
                return ((ulong)sv.data[0] << 32) | (ulong)(sv.data[1] & 0xFFFFFFFF);
            default:
                return null;
        }
    }
}
