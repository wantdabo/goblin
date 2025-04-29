using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Goblin.Sys.Lobby.View;
using Queen.Protocols;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Gameplay
{
    public class GameplayMatchingView : UIBaseView
    {
        protected override string res => "Gameplay/GameplayMatchingView";
        public override UILayer layer => UILayer.UIAlert;
        
        private Dropdown selectHeroDropdown { get; set; }
        private GameObject matchingAlertGo { get; set; }
        private Text matchingDots { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
            engine.net.Recv<S2CStartMatchingMsg>(OnS2CStartMatching);
            engine.net.Recv<S2CEndMatchingMsg>(OnS2CEndMatching);
            engine.net.Recv<S2CStartGameMsg>(OnS2CStartGame);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
            engine.net.UnRecv<S2CStartMatchingMsg>(OnS2CStartMatching);
            engine.net.UnRecv<S2CEndMatchingMsg>(OnS2CEndMatching);
            engine.net.UnRecv<S2CStartGameMsg>(OnS2CStartGame);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            selectHeroDropdown = engine.u3dkit.SeekNode<Dropdown>(gameObject, "SelectHeroDropdown");
            matchingAlertGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "MatchingAlert");
            matchingDots = engine.u3dkit.SeekNode<Text>(gameObject, "MatchingDots");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("MatchingBtn", (e) =>
            {
                var username = engine.u3dkit.SeekNode<InputField>(gameObject, "UserName").text;

                engine.net.Send(new C2SLoginMsg
                {
                    username = username
                });
                
                engine.net.Send(new C2SStartMatchingMsg
                {
                    hero = engine.cfg.location.HeroInfos.DataList[selectHeroDropdown.value].Id,
                });
            });
            
            AddUIEventListener("CancelMatchingBtn", (e) =>
            {
                engine.net.Send(new C2SEndMatchingMsg());
            });
            
            AddUIEventListener("ExitBtn", (e) =>
            {
                engine.net.Disconnect();
                engine.gameui.Close(this);
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

        private float elapsed = 0;
        private void OnFixedTick(FixedTickEvent e)
        {
            if (false == matchingAlertGo.activeSelf) return;
            elapsed += e.tick;
            if (elapsed < 0.5f)
            {
                return;
            }
            elapsed -= 0.5f;

            matchingDots.text = matchingDots.text.Length > 3 ? "." : matchingDots.text + ".";
        }
        
        private void OnS2CStartMatching(S2CStartMatchingMsg msg)
        {
            matchingAlertGo.SetActive(true);
            matchingDots.text = ".";
        }
        
        private void OnS2CEndMatching(S2CEndMatchingMsg msg)
        {
            matchingAlertGo.SetActive(false);
        }
        
        private void OnS2CStartGame(S2CStartGameMsg msg)
        {
            GPData data = new GPData();
            data.id = msg.data.id;
            data.seat = msg.seat;
            var sdata = new GPStageData();
            sdata.seed = msg.data.sdata.seed;
            sdata.players = new GPPlayerData[msg.data.sdata.players.Length];
            for (int i = 0; i < msg.data.sdata.players.Length; i++)
            {
                sdata.players[i] = new GPPlayerData
                {
                    seat = msg.data.sdata.players[i].seat,
                    hero = msg.data.sdata.players[i].hero,
                    position = new GPVector3
                    {
                        x = msg.data.sdata.players[i].position.x,
                        y = msg.data.sdata.players[i].position.y,
                        z = msg.data.sdata.players[i].position.z
                    },
                    euler = new GPVector3
                    {
                        x = msg.data.sdata.players[i].euler.x,
                        y = msg.data.sdata.players[i].euler.y,
                        z = msg.data.sdata.players[i].euler.z
                    },
                    scale = new GPVector3
                    {
                        x = msg.data.sdata.players[i].scale.x,
                        y = msg.data.sdata.players[i].scale.y,
                        z = msg.data.sdata.players[i].scale.z
                    }
                };
            }
            data.sdata = sdata;
            engine.proxy.gameplay.Load<LockstepDirector>(data);
            engine.gameui.Close<LobbyView>();
            engine.gameui.Open<GameplayView>();
            engine.gameui.Close(this);
            engine.proxy.gameplay.director.StartGame();
        }
    }
}