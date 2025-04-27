using Queen.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Common.MDB;

/// <summary>
/// 内存数据库操作员
/// </summary>
public class MDBO : Comp
{
    /// <summary>
    /// DB 主机
    /// </summary>
    private string dbhost { get; set; }
    /// <summary>
    /// DB 端口
    /// </summary>
    private int dbport { get; set; }
    /// <summary>
    /// DB 密码
    /// </summary>
    private string dbpwd { get; set; }
    /// <summary>
    /// Redis 连接
    /// </summary>
    private ConnectionMultiplexer connect { get; set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        connect = ConnectionMultiplexer.Connect($"{dbhost}:{dbport},password={dbpwd},abortConnect=false");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        connect.Close();
        connect = null;
    }

    /// <summary>
    /// 配置数据库
    /// </summary>
    /// <param name="dbhost">DB 主机</param>
    /// <param name="dbport">DB 端口</param>
    /// <param name="dbpwd">DB 密码</param>
    public void Initialize(string dbhost, int dbport, string dbpwd)
    {
        this.dbhost = dbhost;
        this.dbport = dbport;
        this.dbpwd = dbpwd;
    }

    /// <summary>
    /// 数据存在
    /// </summary>
    /// <param name="key">KEY</param>
    /// <returns>YES/NO</returns>
    public bool Exists(string key)
    {
        var db = connect.GetDatabase();

        return db.KeyExists(key);
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="key">KEY</param>
    /// <param name="value">VALUE</param>
    /// <returns>YES/NO</returns>
    public bool Get(string key, out string value)
    {
        value = default;
        var db = connect.GetDatabase();

        if (false == Exists(key)) return false;

        value = db.StringGet(key);

        return true;
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="key">KEY</param>
    /// <param name="value">VALUE</param>
    /// <returns>YES/NO</returns>
    public bool Set(string key, string value)
    {
        var db = connect.GetDatabase();

        return db.StringSet(key, value);
    }

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="key">KEY</param>
    /// <returns>YES/NO</returns>
    public bool Del(string key)
    {
        var db = connect.GetDatabase();

        return db.KeyDelete(key);
    }
}
