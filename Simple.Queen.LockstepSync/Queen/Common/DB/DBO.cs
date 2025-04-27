using MongoDB.Bson;
using MongoDB.Driver;
using Queen.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Common.DB;

/// <summary>
/// 数据库操作员
/// </summary>
public class DBO : Comp
{
    /// <summary>
    /// DB 主机
    /// </summary>
    public string dbhost { get; private set; }
    /// <summary>
    /// DB 端口
    /// </summary>
    public int dbport { get; private set; }
    /// <summary>
    /// DB 身份校验
    /// </summary>
    public bool dbauth { get; private set; }
    /// <summary>
    /// DB 用户名
    /// </summary>
    public string dbuser { get; private set; }
    /// <summary>
    /// DB 密码
    /// </summary>
    public string dbpwd { get; private set; }
    /// <summary>
    /// DB 名字
    /// </summary>
    public string dbname { get; private set; }
    /// <summary>
    /// Mongo 连接
    /// </summary>
    private MongoClient connect;

    protected override void OnCreate()
    {
        base.OnCreate();
        if (dbauth)
            connect = new MongoClient($"mongodb://{dbuser}:{dbpwd}@{dbhost}:{dbport}/{dbname}");
        else
            connect = new MongoClient($"mongodb://{dbhost}:{dbport}/{dbname}");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        connect = null;
    }

    /// <summary>
    /// 配置数据库
    /// </summary>
    /// <param name="dbhost">DB 主机</param>
    /// <param name="dbport">DB 端口</param>
    /// <param name="dbauth">DB 身份校验</param>
    /// <param name="dbuser">DB 用户名</param>
    /// <param name="dbpwd">DB 密码</param>
    /// <param name="dbname">DB 名字</param>
    public void Initialize(string dbhost, int dbport, bool dbauth, string dbuser, string dbpwd, string dbname)
    {
        this.dbhost = dbhost;
        this.dbport = dbport;
        this.dbauth = dbauth;
        this.dbuser = dbuser;
        this.dbpwd = dbpwd;
        this.dbname = dbname;
    }

    /// <summary>
    /// Mongo 新增
    /// </summary>
    /// <typeparam name="T">数据类型（需要与 Mongo 集合字段对上）</typeparam>
    /// <param name="name">集合名字</param>
    /// <param name="value">数据</param>
    /// <returns>YES/NO</returns>
    public bool Insert<T>(string name, T value) where T : DBValue<T>
    {
        var database = connect.GetDatabase(dbname);
        var collection = database.GetCollection<T>(name);
        collection.InsertOne(value);

        return true;
    }

    /// <summary>
    /// Mongo 删除
    /// </summary>
    /// <typeparam name="T">数据类型（需要与 Mongo 集合字段对上）</typeparam>
    /// <param name="name">集合名字</param>
    /// <param name="filter">过滤器</param>
    /// <returns>YES/NO</returns>
    public bool Delete<T>(string name, FilterDefinition<T> filter) where T : DBValue<T>
    {
        var database = connect.GetDatabase(dbname);
        var collection = database.GetCollection<T>(name);
        var result = collection.DeleteOne(filter);

        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Mongo 更新
    /// </summary>
    /// <typeparam name="T">数据类型（需要与 Mongo 集合字段对上）</typeparam>
    /// <param name="name">集合名字</param>
    /// <param name="filter">过滤器</param>
    /// <param name="value">数据</param>
    /// <returns>YES/NO</returns>
    public bool Update<T>(string name, FilterDefinition<T> filter, T value) where T : DBValue<T>, new()
    {
        if (false == Find(name, filter, out var values)) return false;

        var val = values.First();
        val.Set(value);
        var database = connect.GetDatabase(dbname);
        var collection = database.GetCollection<T>(name);
        var result = collection.ReplaceOne(filter, val);

        return result.ModifiedCount > 0;
    }
    
    /// <summary>
    /// Mongo 批量更新
    /// </summary>
    /// <param name="name">集合名字</param>
    /// <param name="updates">更新的数据</param>
    /// <typeparam name="T">数据类型（需要与 Mongo 集合字段对上）</typeparam>
    /// <returns>YES/NO</returns>
    public bool UpdateMany<T>(string name, List<WriteModel<T>> updates) where T : DBValue, new()
    {
        var database = connect.GetDatabase(dbname);
        var collection = database.GetCollection<T>(name);
        var result = collection.BulkWrite(updates);

        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Mongo 覆盖（如有数据，将会更新，否则便新增）
    /// </summary>
    /// <typeparam name="T">数据类型（需要与 Mongo 集合字段对上）</typeparam>
    /// <param name="name">集合名字</param>
    /// <param name="filter">过滤器</param>
    /// <param name="value">数据</param>
    /// <returns>YES/NO</returns>
    public bool Replace<T>(string name, FilterDefinition<T> filter, T value) where T : DBValue<T>, new()
    {
        if (false == Find(name, filter, out var values)) return Insert(name, value);

        return Update(name, filter, value);
    }

    /// <summary>
    /// Mongo 查询
    /// </summary>
    /// <typeparam name="T">数据类型（需要与 Mongo 集合字段对上）</typeparam>
    /// <param name="name">集合名字</param>
    /// <param name="filter">过滤器</param>
    /// <param name="values">数据集合</param>
    /// <returns>YES/NO</returns>
    public bool Find<T>(string name, FilterDefinition<T> filter, out List<T> values) where T : DBValue<T>, new()
    {
        var database = connect.GetDatabase(dbname);
        var collection = database.GetCollection<T>(name);
        var result = collection.FindSync(filter);
        values = result.ToList();

        return values.Count > 0;
    }
}
