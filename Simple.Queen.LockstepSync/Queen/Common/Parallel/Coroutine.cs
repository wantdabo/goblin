using Queen.Common.Parallel.Instructions;

namespace Queen.Common.Parallel;

/// <summary>
/// 协程
/// </summary>
public class Coroutine
{
    /// <summary>
    /// ID
    /// </summary>
    public uint id { get; private set; }
    /// <summary>
    /// 协程驱动器
    /// </summary>
    private CoroutineScheduler scheduler { get; set; }
    /// <summary>
    /// 协程工作中
    /// </summary>
    public bool working { get; private set; }
    /// <summary>
    /// 逻辑分片
    /// </summary>
    private IEnumerator<Instruction> enumerator { get; set; }

    /// <summary>
    /// 设置参数
    /// </summary>
    /// <param name="id">协程 ID</param>
    /// <param name="scheduler">协程驱动</param>
    /// <param name="enumerator">逻辑分片</param>
    public void Reset(uint id, CoroutineScheduler scheduler, IEnumerator<Instruction> enumerator)
    {
        this.id = id;
        this.scheduler = scheduler;
        this.enumerator = enumerator;
        this.working = false;
    }

    /// <summary>
    /// 协程指令就绪检查
    /// </summary>
    /// <returns>YES/NO</returns>
    public bool Ready()
    {
        if (null == enumerator.Current) return true;

        return enumerator.Current.finished;
    }

    /// <summary>
    /// 驱动协程指令
    /// </summary>
    /// <param name="tick">tick</param>
    public void Update(float tick)
    {
        if (null == enumerator.Current) return;
        enumerator.Current.Update(tick);
    }

    /// <summary>
    /// 执行协程
    /// </summary>
    /// <returns>YES/NO (true 表示逻辑分片未结束/ 否则就是逻辑分片已结束)</returns>
    public bool Execute()
    {
        working = enumerator.MoveNext();

        return working;
    }
}
