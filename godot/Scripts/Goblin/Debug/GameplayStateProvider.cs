using System.Collections.Generic;
using System.Text.Json.Nodes;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Debug;

/// <summary>
/// Gameplay 层状态提供者——纯读 Stage 公共 API，不写任何数据。
/// </summary>
public class GameplayStateProvider : IStateProvider
{
    private Stage stage { get; set; }

    private static Dictionary<byte, string> statenames { get; } = new()
    {
        { STATE_DEFINE.NONE, "NONE" },
        { STATE_DEFINE.BORN, "BORN" },
        { STATE_DEFINE.DEATH, "DEATH" },
        { STATE_DEFINE.IDLE, "IDLE" },
        { STATE_DEFINE.MOVE, "MOVE" },
        { STATE_DEFINE.JUMP, "JUMP" },
        { STATE_DEFINE.FALL, "FALL" },
        { STATE_DEFINE.CASTING, "CASTING" },
        { STATE_DEFINE.HITSTUN, "HITSTUN" },
        { STATE_DEFINE.ROLL, "ROLL" },
    };

    private static Dictionary<byte, string> actortypenames { get; } = new()
    {
        { ACTOR_DEFINE.NONE, "NONE" },
        { ACTOR_DEFINE.STAGE, "STAGE" },
        { ACTOR_DEFINE.FLOW, "FLOW" },
        { ACTOR_DEFINE.HERO, "HERO" },
        { ACTOR_DEFINE.MAGIC, "MAGIC" },
        { ACTOR_DEFINE.BUFF, "BUFF" },
        { ACTOR_DEFINE.ENEMY, "ENEMY" },
    };

    private static Dictionary<byte, string> slotkeynames { get; } = new()
    {
        { ANIM_DEFINE.SLOT_TYPE_STATE, "STATE" },
        { ANIM_DEFINE.SLOT_TYPE_NAMED, "NAMED" },
        { ANIM_DEFINE.SLOT_TYPE_OVERRIDE, "OVERRIDE" },
    };

    private static Dictionary<byte, string> layernames { get; } = new()
    {
        { ANIM_DEFINE.LAYER_FULLBODY, "全身" },
        { ANIM_DEFINE.LAYER_UPPER, "上半身" },
        { ANIM_DEFINE.LAYER_LOWER, "下半身" },
    };

    private static Dictionary<ushort, string> attrnames { get; } = new()
    {
        { ATTRIBUTE_DEFINE.HP, "HP" },
        { ATTRIBUTE_DEFINE.MAXHP, "MAXHP" },
        { ATTRIBUTE_DEFINE.MOVESPEED, "MOVESPEED" },
        { ATTRIBUTE_DEFINE.ATTACK, "ATTACK" },
    };

    public GameplayStateProvider(Stage s)
    {
        stage = s;
    }

    public JsonObject Snapshot()
    {
        JsonObject root = new()
        {
            ["frame"] = stage.frame,
            ["elapsed"] = FpToFloat(stage.elapsed),
            ["timescale"] = FpToFloat(stage.timescale),
            ["state"] = stage.state.ToString(),
            ["actors"] = GetActorSummaries(),
        };
        return root;
    }

    public JsonArray GetActorSummaries()
    {
        JsonArray arr = new();
        StageInfo stageinfo = stage.GetBehaviorInfo<StageInfo>(stage.sa);
        if (null == stageinfo) return arr;

        foreach (ulong actorid in stageinfo.actors)
        {
            if (actorid == stage.sa) continue;

            JsonObject summary = new() { ["id"] = actorid };

            if (stage.SeekBehaviorInfo<TagInfo>(actorid, out TagInfo taginfo) &&
                taginfo.tags.TryGetValue(TAG_DEFINE.ACTOR_TYPE, out long actortype))
            {
                summary["type"] = actortypenames.GetValueOrDefault((byte)actortype, actortype.ToString());
            }

            if (stage.SeekBehaviorInfo<StateMachineInfo>(actorid, out StateMachineInfo sminfo))
            {
                summary["state"] = statenames.GetValueOrDefault(sminfo.current, sminfo.current.ToString());
            }

            arr.Add(summary);
        }
        return arr;
    }

    public JsonObject GetActor(ulong actorid)
    {
        JsonObject node = new()
        {
            ["id"] = actorid,
        };

        if (stage.SeekBehaviorInfo<TagInfo>(actorid, out TagInfo taginfo))
        {
            JsonObject tags = new();
            foreach (KeyValuePair<ushort, long> kv in taginfo.tags)
            {
                if (kv.Key == TAG_DEFINE.ACTOR_TYPE)
                    tags["ACTOR_TYPE"] = actortypenames.GetValueOrDefault((byte)kv.Value, kv.Value.ToString());
                else
                    tags[kv.Key.ToString()] = kv.Value;
            }
            node["tags"] = tags;
        }

        if (stage.SeekBehaviorInfo<SpatialInfo>(actorid, out SpatialInfo spatialinfo))
        {
            node["spatial"] = new JsonObject
            {
                ["position"] = V3ToJson(spatialinfo.position),
                ["euler"] = V3ToJson(spatialinfo.euler),
                ["scale"] = FpToFloat(spatialinfo.scale),
            };
        }

        if (stage.SeekBehaviorInfo<StateMachineInfo>(actorid, out StateMachineInfo sminfo))
        {
            node["state_machine"] = new JsonObject
            {
                ["current"] = statenames.GetValueOrDefault(sminfo.current, sminfo.current.ToString()),
                ["last"] = statenames.GetValueOrDefault(sminfo.last, sminfo.last.ToString()),
                ["delaybreak"] = FpToFloat(sminfo.delaybreak),
                ["usedelaybreak"] = sminfo.usedelaybreak,
            };
        }

        if (actorid != stage.sa)
        {
            node["attributes"] = BuildAttrJson(actorid);
        }

        if (stage.SeekBehaviorInfo<FlowInfo>(actorid, out FlowInfo flowinfo) && flowinfo.active)
        {
            node["flow"] = BuildFlowJson(flowinfo);
        }

        if (stage.SeekBehaviorInfo<FacadeInfo>(actorid, out FacadeInfo facadeinfo))
        {
            node["facade"] = BuildFacadeJson(facadeinfo);
        }

        return node;
    }

    public JsonArray GetStateMachines()
    {
        JsonArray arr = new();
        StageInfo stageinfo = stage.GetBehaviorInfo<StageInfo>(stage.sa);
        if (null == stageinfo) return arr;

        foreach (ulong actorid in stageinfo.actors)
        {
            if (actorid == stage.sa) continue;
            if (false == stage.SeekBehaviorInfo<StateMachineInfo>(actorid, out StateMachineInfo sminfo)) continue;
            if (false == stage.SeekBehaviorInfo<TagInfo>(actorid, out TagInfo taginfo)) continue;

            taginfo.tags.TryGetValue(TAG_DEFINE.ACTOR_TYPE, out long actortype);

            arr.Add(new JsonObject
            {
                ["id"] = actorid,
                ["type"] = actortypenames.GetValueOrDefault((byte)actortype, actortype.ToString()),
                ["current"] = statenames.GetValueOrDefault(sminfo.current, sminfo.current.ToString()),
                ["last"] = statenames.GetValueOrDefault(sminfo.last, sminfo.last.ToString()),
            });
        }
        return arr;
    }

    public JsonObject GetFlow(ulong actorid)
    {
        if (false == stage.SeekBehaviorInfo<FlowInfo>(actorid, out FlowInfo flowinfo) || false == flowinfo.active)
            return new JsonObject { ["active"] = false };

        return BuildFlowJson(flowinfo);
    }

    public JsonObject GetAttributes(ulong actorid)
    {
        return BuildAttrJson(actorid);
    }

    // ---- helpers ----

    private static JsonObject BuildFacadeJson(FacadeInfo info)
    {
        JsonObject facade = new()
        {
            ["animstate"] = statenames.GetValueOrDefault(info.animstate, info.animstate.ToString()),
            ["animname"] = info.animhash != 0 ? $"0x{info.animhash:X8}" : null,
            ["animelapsed"] = FpToFloat(info.animelapsed),
            ["animticktype"] = info.animticktype == ANIM_DEFINE.TICK_AUTOMATIC ? "AUTOMATIC" : "MANUAL",
        };

        JsonArray slots = new();
        foreach (var slot in info.animslots)
        {
            var slottype = ANIM_DEFINE.GetSlotType(slot.key);
            var keydisplay = slotkeynames.GetValueOrDefault(slottype, slottype.ToString());
            slots.Add(new JsonObject
            {
                ["key"] = $"{keydisplay}:L{slot.layer}",
                ["priority"] = slot.priority,
                ["active"] = slot.active,
                ["animstate"] = statenames.GetValueOrDefault(slot.animstate, slot.animstate.ToString()),
                ["animname"] = slot.animhash != 0 ? $"0x{slot.animhash:X8}" : null,
                ["layer"] = layernames.GetValueOrDefault(slot.layer, $"L{slot.layer}"),
                ["elapsed"] = FpToFloat(slot.elapsed),
                ["istransient"] = slot.istransient,
                ["duration"] = slot.istransient ? FpToFloat(slot.duration) : 0,
            });
        }
        facade["animslots"] = slots;

        // 逐层预告 winner
        facade["winners"] = BuildLayerWinners(info);

        return facade;
    }

    private static JsonArray BuildLayerWinners(FacadeInfo info)
    {
        JsonArray winners = new();
        for (byte l = 0; l < ANIM_DEFINE.LAYER_MAX; l++)
        {
            AnimationSlot? winner = null;
            foreach (var slot in info.animslots)
            {
                if (false == slot.active || slot.layer != l) continue;
                winner = slot;
                break;
            }
            if (null == winner && 0 != l) continue;

            string statename = null != winner
                ? statenames.GetValueOrDefault(winner.animstate, winner.animstate.ToString())
                : statenames.GetValueOrDefault(info.animstate, info.animstate.ToString()) + " (fallback)";
            string? hashstr = null != winner && 0 != winner.animhash
                ? $"0x{winner.animhash:X8}"
                : (0 != info.animhash ? $"0x{info.animhash:X8}" : null);

            winners.Add(new JsonObject
            {
                ["layer"] = layernames.GetValueOrDefault(l, $"L{l}"),
                ["animstate"] = statename,
                ["animhash"] = hashstr,
                ["elapsed"] = FpToFloat(null != winner ? winner.elapsed : info.animelapsed),
            });
        }
        return winners;
    }

    private static JsonObject BuildFlowJson(FlowInfo info)
    {
        JsonArray pipelinearr = new();
        foreach (uint p in info.pipelines)
            pipelinearr.Add(p);

        return new JsonObject
        {
            ["active"] = info.active,
            ["owner"] = info.owner,
            ["timeline"] = info.timeline,
            ["length"] = info.length,
            ["pipelines"] = pipelinearr,
            ["doing_count"] = info.doings.Count,
        };
    }

    private JsonObject BuildAttrJson(ulong actorid)
    {
        JsonObject attrnode = new();

        AttributeBucketInfo attrinfo = stage.GetBehaviorInfo<AttributeBucketInfo>(stage.sa);
        if (null == attrinfo || false == attrinfo.attributes.TryGetValue(actorid, out Dictionary<ushort, int> attrs))
            return attrnode;

        foreach (KeyValuePair<ushort, int> kv in attrs)
        {
            if (1 == kv.Key % 2)
            {
                ushort basekey = (ushort)((kv.Key - 1) / 2);
                string name = attrnames.GetValueOrDefault(basekey, $"attr_{basekey}");
                int value = kv.Value;
                ushort scalekey = (ushort)(kv.Key + 1);

                if (attrs.TryGetValue(scalekey, out int scale) && 1000 != scale)
                {
                    long effective = (long)value * scale / 1000;
                    attrnode[name] = new JsonObject
                    {
                        ["value"] = effective,
                        ["raw"] = value,
                        ["scale"] = scale,
                    };
                }
                else
                {
                    attrnode[name] = value;
                }
            }
        }
        return attrnode;
    }

    private static JsonObject V3ToJson(FPVector3 v)
    {
        return new JsonObject
        {
            ["x"] = FpToFloat(v.x),
            ["y"] = FpToFloat(v.y),
            ["z"] = FpToFloat(v.z),
        };
    }

    internal static double FpToFloat(FP fp) => fp.AsFloat();
}
