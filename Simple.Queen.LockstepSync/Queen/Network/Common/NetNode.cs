using Queen.Core;
using Queen.Protocols.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Queen.Network.Common;

/// <summary>
/// 网络节点
/// </summary>
public abstract class NetNode : Comp
{
    /// <summary>
    /// 消息包结构
    /// </summary>
    private struct NetPackage
    {
        /// <summary>
        /// 通信渠道
        /// </summary>
        public NetChannel channel;
        /// <summary>
        /// 消息类型
        /// </summary>
        public Type msgType;
        /// <summary>
        /// 消息
        /// </summary>
        public INetMessage msg;
    }

    /// <summary>
    /// 是否自动通知消息
    /// </summary>
    protected bool notify { get; private set; }

    /// <summary>
    /// 最大网络收发包每秒
    /// </summary>
    protected int maxpps { get; private set; }

    /// <summary>
    /// 注册消息回调集合
    /// </summary>
    private ConcurrentDictionary<Type, List<Delegate>> messageActionDict = new();
    /// <summary>
    /// 网络消息包缓存
    /// </summary>
    private ConcurrentQueue<NetPackage> netpackages = new();

    /// <summary>
    /// 通信渠道集合
    /// </summary>
    private ConcurrentDictionary<string, NetChannel> channelDict = new();

    /// <summary>
    /// PPS 记录
    /// </summary>
    private ConcurrentDictionary<string, uint> ppscntDict = new();

    /// <summary>
    /// PPS 计时器
    /// </summary>
    private uint ppsTimingId = 0;

    protected override void OnCreate()
    {
        base.OnCreate();
        ppsTimingId = engine.ticker.Timing((t) =>
        {
            ppscntDict.Clear();
        }, 1, -1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        engine.ticker.StopTimer(ppsTimingId);
    }

    /// <summary>
    /// 创建网络节点
    /// </summary>
    /// <param name="notify">是否自动通知消息（子线程）</param>
    /// <param name="maxpps">最大网络收发包每秒</param>
    protected void Initialize(bool notify, int maxpps)
    {
        this.notify = notify;
        this.maxpps = maxpps;
    }

    /// <summary>
    /// 获取通信渠道
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="channel">通信渠道</param>
    /// <returns>YES/NO</returns>
    public bool GetChannel(string id, out NetChannel channel)
    {
        return channelDict.TryGetValue(id, out channel);
    }

    /// <summary>
    /// 移除通信渠道
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>YES/NO</returns>
    public bool RmvChannel(string id)
    {
        return channelDict.Remove(id, out var val);
    }

    /// <summary>
    /// 添加通信渠道
    /// </summary>
    /// <param name="channel">通信渠道</param>
    /// <returns>YES/NO</returns>
    public bool AddChannel(NetChannel channel)
    {
        if (channelDict.ContainsKey(channel.id)) return false;

        channelDict.TryAdd(channel.id, channel);

        return true;
    }

    /// <summary>
    /// 获取 PPS
    /// </summary>
    /// <param name="channel">通信渠道</param>
    /// <returns>该通信渠道 PPS</returns>
    private uint GetPPS(NetChannel channel)
    {
        if (false == ppscntDict.TryGetValue(channel.id, out var cnt)) return 0;

        return cnt;
    }

    /// <summary>
    /// PPS 计数
    /// </summary>
    /// <param name="channel">通信渠道</param>
    private void PPSCounter(NetChannel channel)
    {
        uint cnt = 0;
        if (ppscntDict.TryGetValue(channel.id, out cnt)) ppscntDict.Remove(channel.id, out var val);
        cnt++;
        ppscntDict.TryAdd(channel.id, cnt);
    }

    /// <summary>
    /// 注销消息接收
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="action">回调方法</param>
    public void UnRecv<T>(Action<NetChannel, T> action) where T : INetMessage
    {
        if (false == messageActionDict.TryGetValue(typeof(T), out var actions)) return;
        if (false == actions.Contains(action)) return;

        actions.Remove(action);
    }

    /// <summary>
    /// 注册消息接收
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="action">回调方法</param>
    public void Recv<T>(Action<NetChannel, T> action) where T : INetMessage
    {
        if (false == messageActionDict.TryGetValue(typeof(T), out var actions))
        {
            actions = new();
            messageActionDict.TryAdd(typeof(T), actions);
        }

        if (actions.Contains(action)) return;
        actions.Add(action);
    }

    /// <summary>
    /// 消息包入队
    /// </summary>
    /// <param name="channel">通信渠道</param>
    /// <param name="msgType">消息类型</param>
    /// <param name="msg">消息</param>
    private void EnqueuePackage(NetChannel channel, Type msgType, INetMessage msg)
    {
        netpackages.Enqueue(new NetPackage { channel = channel, msgType = msgType, msg = msg });
        if (notify) Notify();
    }

    /// <summary>
    /// 连接消息
    /// </summary>
    /// <param name="channel">通信渠道</param>
    protected void EmitConnectEvent(NetChannel channel)
    {
        EnqueuePackage(channel, typeof(NodeConnectMsg), new NodeConnectMsg { });
    }

    /// <summary>
    /// 断开连接消息
    /// </summary>
    /// <param name="channel">通信渠道</param>
    protected void EmitDisconnectEvent(NetChannel channel)
    {
        EnqueuePackage(channel, typeof(NodeDisconnectMsg), new NodeDisconnectMsg { });
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="channel">通信渠道</param>
    /// <param name="data">数据二进制</param>
    protected void EmitReceiveEvent(NetChannel channel, byte[] data)
    {
        // 判断 PPS 是否超过服务器设定
        // maxpps : settings.json 中定义
        var pps = GetPPS(channel);
        if (pps >= maxpps)
        {
            channel.Send(new NodeErrorMsg { code = NEC.PPS_LIMIT });

            return;
        }

        // 解包
        if (false == ProtoPack.UnPack(data, out var msgType, out var msg)) return;
        EnqueuePackage(channel, msgType, msg);

        // PPS 计数
        PPSCounter(channel);
    }

    /// <summary>
    /// 消息通知
    /// </summary>
    public void Notify()
    {
        while (netpackages.TryDequeue(out var package))
        {
            if (false == messageActionDict.TryGetValue(package.msgType, out var actions)) continue;
            for (int i = actions.Count - 1; i >= 0; i--) actions[i]?.DynamicInvoke(package.channel, package.msg);
        }
    }
}