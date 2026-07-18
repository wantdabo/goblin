using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;

namespace Goblin.Debug;

/// <summary>
/// 调试 HTTP 服务——在后台线程监听，对外暴露状态查询和 Tick 控制 API。
/// 状态读取通过 DebugServer.stateprovider，控制指令通过 DebugServer。
/// </summary>
public class DebugHttpServer
{
    private HttpListener listener { get; set; }
    private Thread? httpthread { get; set; }
    private bool running { get; set; }
    private const int DefaultPort = 9876;

    private DebugServer debug { get; set; }

    public DebugHttpServer(DebugServer debug, int port = DefaultPort)
    {
        this.debug = debug;
        listener = new HttpListener();
        listener.Prefixes.Add($"http://+:{port}/");
    }

    public void Start()
    {
        running = true;
        httpthread = new Thread(HttpLoop)
        {
            IsBackground = true,
            Name = "GoblinDebugHTTP",
        };
        listener.Start();
        httpthread.Start();
    }

    public void Stop()
    {
        running = false;
        try { listener?.Stop(); } catch { }
        try { listener?.Close(); } catch { }
        httpthread?.Join(1000);
    }

    private void HttpLoop()
    {
        while (running && listener.IsListening)
        {
            try
            {
                HttpListenerContext ctx = listener.GetContext();
                ThreadPool.QueueUserWorkItem(_ => HandleRequest(ctx));
            }
            catch (HttpListenerException) { break; }
            catch (Exception) { }
        }
    }

    private void HandleRequest(HttpListenerContext ctx)
    {
        try
        {
            string path = ctx.Request.Url!.AbsolutePath.TrimEnd('/');
            string method = ctx.Request.HttpMethod;
            System.Collections.Specialized.NameValueCollection query = ctx.Request.QueryString;

            string json;
            int statuscode = 200;

            switch (method)
            {
                case "GET":
                    json = HandleGet(path, query);
                    break;
                case "POST":
                    json = HandlePost(path, ctx);
                    break;
                case "DELETE":
                    json = HandleDelete(path);
                    break;
                default:
                    json = Error("method not allowed");
                    statuscode = 405;
                    break;
            }

            WriteResponse(ctx, json, statuscode);
        }
        catch (Exception e)
        {
            WriteResponse(ctx, Error(e.Message), 500);
        }
    }

    private string HandleGet(string path, System.Collections.Specialized.NameValueCollection query)
    {
        IStateProvider? state = debug.stateprovider;
        if (null == state && path != "/status")
            return Error("no game active");

        switch (path)
        {
            case "/status": return Ok(debug.GetStatus());
            case "/state": return Ok(state!.Snapshot());
            case "/actors": return Ok(state!.GetActorSummaries());
            case "/state_machines": return Ok(state!.GetStateMachines());

            default:
                if (path.StartsWith("/actor/"))
                {
                    string idstr = path.Substring("/actor/".Length);
                    if (ulong.TryParse(idstr, out ulong actorid))
                        return Ok(state!.GetActor(actorid));
                    return Error("invalid actor id");
                }
                if (path.StartsWith("/flow/"))
                {
                    string idstr = path.Substring("/flow/".Length);
                    if (ulong.TryParse(idstr, out ulong actorid))
                        return Ok(state!.GetFlow(actorid));
                    return Error("invalid actor id");
                }
                if (path.StartsWith("/attributes/"))
                {
                    string idstr = path.Substring("/attributes/".Length);
                    if (ulong.TryParse(idstr, out ulong actorid))
                        return Ok(state!.GetAttributes(actorid));
                    return Error("invalid actor id");
                }
                return Error("unknown endpoint");
        }
    }

    private string HandlePost(string path, HttpListenerContext ctx)
    {
        switch (path)
        {
            case "/control/pause":
                debug.Pause();
                return Ok(new JsonObject { ["paused"] = true });

            case "/control/resume":
                debug.Resume();
                return Ok(new JsonObject { ["paused"] = false });

            case "/control/step":
                string nstr = ctx.Request.QueryString["n"];
                int n = string.IsNullOrEmpty(nstr) ? 1 : int.Parse(nstr);
                debug.Step(n);
                return Ok(new JsonObject { ["steps"] = n });

            case "/control/breakpoint":
                string bpbody = ReadBody(ctx);
                Breakpoint? bp = JsonSerializer.Deserialize<Breakpoint>(bpbody);
                if (null == bp) return Error("invalid breakpoint json");
                debug.SetBreakpoint(bp);
                return Ok(new JsonObject { ["breakpoint"] = bp.type.ToString() });

            case "/control/pause_render":
                debug.PauseRender();
                return Ok(new JsonObject { ["rendering_paused"] = true });

            case "/control/resume_render":
                debug.ResumeRender();
                return Ok(new JsonObject { ["rendering_paused"] = false });

            case "/input":
                string inputbody = ReadBody(ctx);
                SimulatedInput? input = JsonSerializer.Deserialize<SimulatedInput>(inputbody);
                if (null == input) return Error("invalid input json");
                debug.InjectInput(input);
                return Ok(new JsonObject { ["injected"] = true });

            default:
                return Error("unknown endpoint");
        }
    }

    private string HandleDelete(string path)
    {
        if ("/control/breakpoint" == path)
        {
            debug.ClearBreakpoint();
            return Ok(new JsonObject { ["breakpoint"] = false });
        }
        return Error("unknown endpoint");
    }

    private static string ReadBody(HttpListenerContext ctx)
    {
        using System.IO.StreamReader reader = new(ctx.Request.InputStream, ctx.Request.ContentEncoding);
        return reader.ReadToEnd();
    }

    private static void WriteResponse(HttpListenerContext ctx, string body, int statuscode)
    {
        ctx.Response.StatusCode = statuscode;
        ctx.Response.ContentType = "application/json; charset=utf-8";
        byte[] buffer = Encoding.UTF8.GetBytes(body);
        ctx.Response.ContentLength64 = buffer.Length;
        ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
        ctx.Response.Close();
    }

    private static string Ok(JsonNode node) => node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
    private static string Error(string msg) => new JsonObject { ["error"] = msg }.ToJsonString();
}
