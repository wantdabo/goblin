using MongoDB.Bson.Serialization.Attributes;
using Queen.Common.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Common.DB;

/// <summary>
/// Mongo 对应数据, 游戏数据
/// </summary>
public class DBDataValue : DBValue<DBDataValue>
{
    /// <summary>
    /// Key
    /// </summary>
    [BsonElement("prefix")]
    public string prefix { get; set; }
    /// <summary>
    /// Value
    /// </summary>
    [BsonElement("value")]
    public byte[] value { get; set; }

    public override void Set(DBDataValue src)
    {
        prefix = src.prefix;
        value = src.value;
    }
}
