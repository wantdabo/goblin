using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Common.DB;

/// <summary>
/// Mongo 对应数据, 玩家信息
/// </summary>
public class DBRoleValue : DBValue<DBRoleValue>
{
    /// <summary>
    /// 玩家 ID
    /// </summary>
    [BsonElement("uuid")]
    public string uuid { get; set; }
    /// <summary>
    /// 玩家昵称
    /// </summary>
    [BsonElement("nickname")]
    public string nickname { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    [BsonElement("username")]
    public string username { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    [BsonElement("password")]
    public string password { get; set; }

    public override void Set(DBRoleValue src)
    {
        uuid = src.uuid;
        nickname = src.nickname;
        username = src.username;
        password = src.password;
    }
}
