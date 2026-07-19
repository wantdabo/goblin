using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Scriptings;

// 重击管线：15 帧（600ms @ 25fps），强度 3000（轻击 1000-1500 的 2-3 倍）
// t=0-600ms  每帧向前位移 80mm，合计 1.2m 前冲
// t=200-500ms 大范围碰撞检测（2000x1500x2000mm），最多命中 5 个目标
// 命中时：受击朝向攻击者反方向弹飞 + 顿帧 + 伤害结算
public class S10020 : Scripting
{
	public override uint id => FLOW_DEFINE.S10020;

	protected override void OnScript()
	{
		Instruct(0, 0, new SoundInstructData
		{
			soundid = 1000002,
		});

		ScriptMachine.Instruct(0, 600, new SpatialPositionData
		{
			type = SPATIAL_DEFINE.POSITION_SELF,
			position = new IntVector3(0, 0, 80),
		}, checkonce: false);

		Instruct(200, 500, new CollisionData
		{
			et = FLOW_DEFINE.ET_MAGIC_OWNER,
			type = COLLISION_DEFINE.COLLISION_TYPE_HURT,
			overlaptype = COLLISION_DEFINE.COLLISION_BOX,
			count = 5,
			offset = new IntVector3(0, 0, 500),
			boxsize = new IntVector3(2000, 1500, 2000),
			usespark = true,
			spark = new SparkData
			{
				influence = SPARK_INSTR_DEFINE.FLOW,
				token = SPARK_INSTR_DEFINE.TOKEN_PIPELINE_GEN,
			},
		});

		Instruct(200, 200, new BeHitData
		{
			et = FLOW_DEFINE.ET_FLOW_HIT,
			uselookatattacker = true,
			usehitmotion = true,
			hitmotiontype = BEHIT_DEFINE.MOTION_ATTACKER_TO_SELF,
			hitmotion = new IntVector3(0, 0, 800),
		});

		Instruct(200, 200, new HitLagData
		{
			et = FLOW_DEFINE.ET_FLOW_HIT,
			type = HIT_LAG_DEFINE.TYPE_INSTANCE,
			strength = 300,
			strengthmax = 300,
			duration = 120,
			durationmax = 120,
		});

		// 命中时伤害结算（由碰撞 spark 触发，确保在 targets 填充后执行）
		ScriptMachine.Instruct(SPARK_INSTR_DEFINE.FLOW, SPARK_INSTR_DEFINE.TOKEN_PIPELINE_GEN, new DamageData
		{
			strength = 3000,
		});
	}
}
