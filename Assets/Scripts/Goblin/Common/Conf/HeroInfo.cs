
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
public sealed partial class HeroInfo : Luban.BeanBase
{
    public HeroInfo(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Attribute = _buf.ReadInt();
        Name = _buf.ReadString();
        Model = _buf.ReadInt();
    }

    public static HeroInfo DeserializeHeroInfo(ByteBuf _buf)
    {
        return new Conf.HeroInfo(_buf);
    }

    /// <summary>
    /// ID
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 属性
    /// </summary>
    public readonly int Attribute;
    /// <summary>
    /// 名字
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 模型
    /// </summary>
    public readonly int Model;
   
    public const int __ID__ = -1793672366;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Attribute:" + Attribute + ","
        + "Name:" + Name + ","
        + "Model:" + Model + ","
        + "}";
    }
}

}

