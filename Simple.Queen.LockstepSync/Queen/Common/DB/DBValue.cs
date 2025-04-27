using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Common.DB;

/// <summary>
/// Mongo 对应数据
/// </summary>
public abstract class DBValue
{
    /// <summary>
    /// MongoDB 文档主键
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    private ObjectId id { get; set; }
}

/// <summary>
/// Mongo 对应数据
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public abstract class DBValue<T> : DBValue where T : DBValue
{
    /// <summary>
    /// 赋值重载
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="src">数据源</param>
    public abstract void Set(T src);
}