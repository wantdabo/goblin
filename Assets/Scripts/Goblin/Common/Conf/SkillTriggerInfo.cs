
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace Conf
{
public sealed partial class SkillTriggerInfo : Luban.BeanBase
{
    public SkillTriggerInfo(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Key = _buf.ReadInt();
    }

    public static SkillTriggerInfo DeserializeSkillTriggerInfo(ByteBuf _buf)
    {
        return new Conf.SkillTriggerInfo(_buf);
    }

    /// <summary>
    /// ID
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 按键
    /// </summary>
    public readonly int Key;
   
    public const int __ID__ = 1494153471;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "key:" + Key + ","
        + "}";
    }
}

}

