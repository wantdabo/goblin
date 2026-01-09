using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// RIL (Runtime Interpolation Layer) 属性标签
/// 用于标记需要生成 RIL 事件接口的接口
/// </summary>

[AttributeUsage(AttributeTargets.Interface)]
public class RIL_EVENTAttribute : Attribute
{
    /// <summary>
    /// RIL 事件标签
    /// </summary>
    public RIL_EVENTAttribute() { }
}
