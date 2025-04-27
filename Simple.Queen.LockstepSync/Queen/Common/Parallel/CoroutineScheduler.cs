﻿using Queen.Common.Parallel.Instructions;
using Queen.Core;

namespace Queen.Common.Parallel;

/// <summary>
/// 协程驱动器
/// </summary>
public class CoroutineScheduler : Comp
{
    /// <summary>
    /// Ticker, 逻辑层 Tick 驱动组件
    /// </summary>
    public Ticker ticker { get; private set; }
    /// <summary>
    /// 协程自增 ID 
    /// </summary>
    private uint incrementId { get; set; }
    /// <summary>
    /// 协程对象对象池, 缓存复用
    /// </summary>
    private readonly Queue<Coroutine> caches = new();
    /// <summary>
    /// 协程对象列表
    /// </summary>
    private readonly List<Coroutine> coroutines = new();

    protected override void OnCreate()
    {
        base.OnCreate();
        ticker.eventor.Listen<TickEvent>(OnTick);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ticker.eventor.UnListen<TickEvent>(OnTick);
    }

    /// <summary>
    /// 初始化参数
    /// </summary>
    /// <param name="ticker">Ticker</param>
    public void Initialize(Ticker ticker)
    {
        this.ticker = ticker;
    }

    /// <summary>
    /// 结束协程
    /// </summary>
    /// <param name="coroutine">协程对象</param>
    public void Shutdown(Coroutine coroutine)
    {
        if (null == coroutines.Find((c) => { return c.id == coroutine.id; })) return;
        caches.Enqueue(coroutine);
        coroutines.Remove(coroutine);
    }

    /// <summary>
    /// 开始协程
    /// </summary>
    /// <param name="enumerator">逻辑分片</param>
    /// <returns>协程对象</returns>
    public Coroutine Execute(IEnumerator<Instruction> enumerator)
    {
        if (false == caches.TryDequeue(out var coroutine)) coroutine = new Coroutine();
        coroutine.Reset(++incrementId, this, enumerator);
        coroutines.Add(coroutine);
        if (false == coroutine.Execute()) Shutdown(coroutine);

        return coroutine;
    }

    /// <summary>
    /// 驱动
    /// </summary>
    /// <param name="tick">tick</param>
    private void Update(float tick)
    {
        for (int i = coroutines.Count - 1; i >= 0; i--)
        {
            var coroutine = coroutines[i];
            if (false == coroutine.Ready())
            {
                coroutine.Update(tick);
                continue;
            }

            if (false == coroutine.Execute()) Shutdown(coroutine);
        }
    }

    private void OnTick(TickEvent e)
    {
        Update(e.tick);
    }
}
