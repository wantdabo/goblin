using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Scriptings;

// 翻滚管线：8 帧（320ms @ 25fps）
// t=0       强制切换到 ROLL 状态（无敌帧由 STATE_DEFINE.PASSES 保证）
// t=0-320ms 每帧向自身前方位移 200mm，合计 1.6m
public class S10010 : Scripting
{
    public override uint id => FLOW_DEFINE.S10010;

    protected override void OnScript()
    {
        Instruct(0, 0, new ChangeStateData
        {
            et = FLOW_DEFINE.ET_MAGIC_OWNER,
            state = STATE_DEFINE.ROLL,
            force = true,
            usedelaybreak = true,
            delaybreak = 320,
        });

        Instruct(0, 0, new SoundInstructData
        {
            et = FLOW_DEFINE.ET_MAGIC_OWNER,
            soundid = 1000001,
        });

        ScriptMachine.Instruct(0, 320, new SpatialPositionData
        {
            et = FLOW_DEFINE.ET_MAGIC_OWNER,
            type = SPATIAL_DEFINE.POSITION_SELF,
            position = new IntVector3(0, 0, 200),
        }, checkonce: false);
    }
}
