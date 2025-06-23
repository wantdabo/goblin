using Goblin.Common;
using Goblin.Sys.Common;
using UnityEngine;

namespace Goblin.Sys.Gameplay.View
{
    /// <summary>
    /// 治疗跳字
    /// </summary>
    public struct CureDanceEvent : IEvent
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Vector3 screenpos { get; set; }
        /// <summary>
        /// 治疗数值
        /// </summary>
        public uint cure { get; set; }
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public ulong from { get; set; }
        /// <summary>
        /// 去向/ActorID
        /// </summary>
        public ulong to { get; set; }
    }

    /// <summary>
    /// 伤害跳字
    /// </summary>
    public struct DamageDanceEvent : IEvent
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Vector3 screenpos { get; set; }
        /// <summary>
        /// 暴击
        /// </summary>
        public bool crit { get; set; }
        /// <summary>
        /// 伤害数值
        /// </summary>
        public int damage { get; set; }
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public ulong from { get; set; }
        /// <summary>
        /// 去向/ActorID
        /// </summary>
        public ulong to { get; set; }
    }
    
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

            var obj = ObjectPool.Get<GameplayDanceObject>("BLOOD_DANCE_DAMAGE");
            if (null == obj)
            {
                var go = GameObject.Instantiate(damageOrgGo, contentGo.transform);
                obj = new GameplayDanceObject(go);
            }
            
            obj.go.transform.position = engine.gameui.uicamera.ScreenToWorldPoint(new Vector3(e.screenpos.x, e.screenpos.y, obj.go.transform.position.z));
            obj.Settings(e.damage);
            obj.go.SetActive(true);
            obj.Play();
            engine.ticker.Timing((t) =>
            {
                obj.go.SetActive(false);
                ObjectPool.Set(obj, "BLOOD_DANCE_DAMAGE");
            }, 0.7f, 1);
        }
    }
}
