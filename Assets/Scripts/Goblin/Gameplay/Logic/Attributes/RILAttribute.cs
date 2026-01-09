using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// RIL (Runtime Interpolation Layer) 属性标签
/// 用于标记需要在运行时插值层中处理的字段或属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class RILAttribute : Attribute
{

    public string RILName { get; set; }
    public bool Once { get; set; }
    public RILAttribute()
    {
        RILName = string.Empty;
        Once = false;
    }

    public RILAttribute(string name)
    {
        RILName = name;
        Once = false;
    }

    public RILAttribute(string name, bool once)
    {
        RILName = name;
        Once = once;
    }
}
