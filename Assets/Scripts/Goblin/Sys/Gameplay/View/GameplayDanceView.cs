using Goblin.Sys.Common;
using UnityEngine;

namespace Goblin.Sys.Gameplay.View
{
    public class GameplayDanceView : UIBaseView
    {
        protected override string res => "Gameplay/GameplayDanceView";
        
        public override UILayer layer => UILayer.UIAlert;

        private GameObject contentGo { get; set; }
        private GameObject damageOrgGo { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.proxy.gameplay.eventor.Listen<RecvCureEvent>(OnRecvCure);
            engine.proxy.gameplay.eventor.Listen<RecvHurtEvent>(OnRecvHurt);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.proxy.gameplay.eventor.UnListen<RecvCureEvent>(OnRecvCure);
            engine.proxy.gameplay.eventor.UnListen<RecvHurtEvent>(OnRecvHurt);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            contentGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "Content");
            damageOrgGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "DamageOrg");
        }
        
        private void OnRecvCure(RecvCureEvent e)
        {
            var obj = engine.pool.Get<GameplayDanceObject>("BLOOD_DANCE_DAMAGE");
            if (null == obj)
            {
                var go = GameObject.Instantiate(damageOrgGo, contentGo.transform);
                obj = new GameplayDanceObject(go);
            }
            obj.go.SetActive(true);
        }
        
        private void OnRecvHurt(RecvHurtEvent e)
        {
        }
    }
}
