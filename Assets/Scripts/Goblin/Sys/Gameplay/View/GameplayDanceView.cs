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
            engine.proxy.gameplay.eventor.Listen<CureDanceEvent>(OnCureDance);
            engine.proxy.gameplay.eventor.Listen<DamageDanceEvent>(OnDamageDance);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.proxy.gameplay.eventor.UnListen<CureDanceEvent>(OnCureDance);
            engine.proxy.gameplay.eventor.UnListen<DamageDanceEvent>(OnDamageDance);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            contentGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "Content");
            damageOrgGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "DamageOrg");
        }

        private void OnCureDance(CureDanceEvent e)
        {
            if (false == engine.proxy.gameplay.dancing) return;
        }

        private void OnDamageDance(DamageDanceEvent e)
        {
            if (false == engine.proxy.gameplay.dancing) return;

            var obj = engine.pool.Get<GameplayDanceObject>("BLOOD_DANCE_DAMAGE");
            if (null == obj)
            {
                var go = GameObject.Instantiate(damageOrgGo, contentGo.transform);
                obj = new GameplayDanceObject(go);
            }
            var screenpos = engine.proxy.gameplay.stage.foc.eyes.camera.WorldToScreenPoint(e.position);
            obj.go.transform.position = engine.gameui.uicamera.ScreenToWorldPoint(new Vector3(screenpos.x, screenpos.y, obj.go.transform.position.z));
            obj.Settings(e.damage);
            obj.go.SetActive(true);
            obj.Play();
            engine.ticker.Timing((t) =>
            {
                obj.go.SetActive(false);
                engine.pool.Set("BLOOD_DANCE_DAMAGE", obj);
            }, 0.7f, 1);
        }
    }
}
