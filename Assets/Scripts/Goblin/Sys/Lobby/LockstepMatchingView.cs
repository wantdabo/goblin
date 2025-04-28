using System.Collections.Generic;
using Goblin.Sys.Common;
using UnityEngine.UI;

namespace Goblin.Sys.Lobby
{
    public class LockstepMatchingView : UIBaseView
    {
        protected override string res => "Lobby/LockstepMatchingView";
        public override UILayer layer => UILayer.UIAlert;
        
        private Dropdown selectHeroDropdown { get; set; }
        
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            selectHeroDropdown = engine.u3dkit.SeekNode<Dropdown>(gameObject, "SelectHeroDropdown");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            selectHeroDropdown.onValueChanged.AddListener((int index) =>
            {
                UnityEngine.Debug.Log(index);
            });
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            var options = new List<Dropdown.OptionData>();
            foreach (var heroinfo in engine.cfg.location.HeroInfos.DataList)
            {
                options.Add(new Dropdown.OptionData
                {
                    text = $"{heroinfo.Id}-{heroinfo.Name}",
                });
            }
            selectHeroDropdown.options = options;
        }
    }
}