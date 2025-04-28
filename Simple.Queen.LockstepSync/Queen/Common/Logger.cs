using Queen.Core;
using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace Queen.Common;

/// <summary>
/// 日志系统
/// </summary>
public class Logger : Comp
{
    private string logdir = $"{Directory.GetCurrentDirectory()}/Res/Logs/";

    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
    }

    /// <summary>
    /// 日志数据结构
    /// </summary>
    private struct LogInfo
    {
        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel ll;
        /// <summary>
        /// 日志时间
        /// </summary>
        public string time;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string message;
        /// <summary>
        /// 日志颜色
        /// </summary>
        public ConsoleColor color;
    }

    /// <summary>
    /// 日志队列
    /// </summary>
    private ConcurrentQueue<string> logInfos = new();

    private StreamWriter writer;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        if (false == Directory.Exists(logdir)) Directory.CreateDirectory(logdir);

        var logFilePath = $"{logdir}Log_{DateTime.Now.ToString("yyyy-MM-dd")}{DateTime.Now.ToLongTimeString().Replace(':', '.')}.txt";
        var fs = File.Open(logFilePath, FileMode.OpenOrCreate);
        writer = new StreamWriter(fs);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// 信息日志
    /// </summary>
    /// <param name="message">日志内容</param>
    public void Info(string message, ConsoleColor color = ConsoleColor.White)
    {
        Log(new LogInfo { ll = LogLevel.Info, time = $"{DateTime.Now.ToString("yyyy-MM-dd")} {DateTime.Now.ToLongTimeString()}", message = message, color = color });
    }

    /// <summary>
    /// 警告日志
    /// </summary>
    /// <param name="message">日志内容</param>
    public void Warn(string message, ConsoleColor color = ConsoleColor.White)
    {
        Log(new LogInfo { ll = LogLevel.Warn, time = $"{DateTime.Now.ToString("yyyy-MM-dd")} {DateTime.Now.ToLongTimeString()}", message = message, color = color });
    }

    /// <summary>
    /// 错误日志
    /// </summary>
    /// <param name="message">日志内容</param>
    public void Error(string message, ConsoleColor color = ConsoleColor.White)
    {
        Log(new LogInfo { ll = LogLevel.Error, time = $"{DateTime.Now.ToString("yyyy-MM-dd")} {DateTime.Now.ToLongTimeString()}", message = message, color = color });
    }

    /// <summary>
    /// 错误日志
    /// </summary>
    /// <param name="e">异常</param>
    public void Error(Exception e, ConsoleColor color = ConsoleColor.White)
    {
        Error($"{e.Message}\n{e.StackTrace}");
    }

    /// <summary>
    /// 错误日志
    /// </summary>
    /// <param name="message">日志内容</param>
    /// <param name="e">异常</param>
    public void Error(string message, Exception e, ConsoleColor color = ConsoleColor.White)
    {
        Error($"{message}\n{e.Message}\n{e.Message}\n{e.StackTrace}");
    }

    /// <summary>
    /// 打印日志
    /// </summary>
    /// <param name="log">日志</param>
    private void Log(LogInfo log)
    {
        var llstr = string.Empty;
        if (LogLevel.Info == log.ll)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            llstr = "[INFO] ";
        }
        else if (LogLevel.Warn == log.ll)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            llstr = "[WARN] ";
        }
        else if (LogLevel.Error == log.ll)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            llstr = "[ERRO] ";
        }

        logInfos.Enqueue($"{llstr}{log.time} {log.message}");

        Console.Write(llstr);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{log.time} ");
        Console.ForegroundColor = log.color;
        Console.Write($"{log.message}");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
    }
}
