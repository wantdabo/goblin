namespace GoblinFramework.Client.Gameplay
{
    public class TestGameStageConf : GameStageConf
    {
    }

    public class TestGameStage : GameStage<TestGameStage, TestGameStageConf>
    {
        public override void OnAnalyze(TestGameStageConf stageConf)
        {
        }
        
        public override void OnPlay()
        {
        }

        public override void OnPause()
        {
        }

        public override void OnGaming(float tick)
        {
        }

        public override void OnEnd()
        {
        }
    }
}