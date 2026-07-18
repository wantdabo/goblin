using Goblin.Core;
using System.Threading.Tasks;

namespace Goblin.Common.GameRes;

public class Location : Comp
{
    public const string soundpath = "res://GameRes/Sound/";
    public const string modelpath = "res://GameRes/Model/";
    public const string animcfgpath = "res://GameRes/AnimCfg/";
    public const string effectpath = "res://GameRes/Effect/";
    public const string uieffectpath = "res://GameRes/UIEffect/";
    public const string uiprefabpath = "res://GameRes/UIPrefab/";
    public const string spritespath = "res://GameRes/UISprite/";
    public const string configpath = "res://GameRes/Raw/Configs/";
    public const string pipelinepath = "res://GameRes/Raw/Pipelines/";

    public byte[] LoadConfigSync(string res)
    {
        return engine.gameres.LoadRawFileSync(configpath + res + ".bytes");
    }

    public string LoadConfigJson(string res)
    {
        return engine.gameres.LoadRawFileText(configpath + res + ".json");
    }

    public async Task<byte[]> LoadConfigAsync(string res)
    {
        return await engine.gameres.LoadRawFileAsync(configpath + res + ".bytes");
    }

    public async Task<string> LoadConfigJsonAsync(string res)
    {
        return await engine.gameres.LoadRawFileTextAsync(configpath + res + ".json");
    }

    public byte[] LoadPipelineSync(string res)
    {
        return engine.gameres.LoadRawFileSync(pipelinepath + res + ".bytes");
    }

    public async Task<byte[]> LoadPipelineAsync(string res)
    {
        return await engine.gameres.LoadRawFileAsync(pipelinepath + res + ".bytes");
    }
}