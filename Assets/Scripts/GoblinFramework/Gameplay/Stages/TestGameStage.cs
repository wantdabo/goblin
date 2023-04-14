namespace GoblinFramework.Gameplay
{
    public class TestGameStageConf : GameStageConf
    {
    }

    public class TestGameStage : GameStage<TestGameStage, TestGameStageConf>
    {
        protected override void OnAnalyze(TestGameStageConf stageConf)
        {
        }
        
        protected override void OnPlay()
        {
        }

        protected override void OnPause()
        {
        }

        protected override void OnGaming(float tick)
        {
        }

        protected override void OnEnd()
        {
        }
    }
}