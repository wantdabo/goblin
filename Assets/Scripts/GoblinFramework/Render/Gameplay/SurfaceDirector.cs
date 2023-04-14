using System;
using System.Collections.Generic;
using GoblinFramework.Common.Events;
using GoblinFramework.Gameplay;
using GoblinFramework.Gameplay.Events;
using GoblinFramework.Render.Common;
using UnityEngine;

namespace GoblinFramework.Render.Gameplay
{
    public class SurfaceDirector : RComp
    {
        private GameStatus mState = GameStatus.End;

        /// <summary>
        /// 关卡状态
        /// </summary>
        public GameStatus state
        {
            get { return mState; }
            private set { mState = value; }
        }

        private GameStage stage;
        private List<Surface> surfaces = new();
        private Dictionary<uint, Surface> surfaceDict = new();

        public void Create(GameStage s)
        {
            stage = s;
            Create();
        }

        public override void Create()
        {
            if (null == stage) throw new Exception("plz use method Create(GameStage s) to create director.");
            base.Create();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            stage.eventor.Listen<GameStatusEvent>(OnGameStatus);
            stage.eventor.Listen<RmvActorEvent>(OnRmvActor);
            stage.eventor.Listen<AddActorEvent>(OnAddActor);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stage.eventor.UnListen<GameStatusEvent>(OnGameStatus);
            stage.eventor.UnListen<RmvActorEvent>(OnRmvActor);
            stage.eventor.UnListen<AddActorEvent>(OnAddActor);
        }
        
        public Surface GetSurface(uint id)
        {
            if (surfaceDict.TryGetValue(id, out var surface)) return surface;

            return null;
        }
        
        private void OnGameStatus(GameStatusEvent e)
        {
            Debug.Log($"OnGameStatus =====>{e.state}");
            
            state = e.state;
        }

        private void OnRmvActor(RmvActorEvent e)
        {
            Debug.Log($"OnRmvActor =====>{e.actor}");
            
            var surface = AddComp<Surface>();
            surface.id = e.actor;
            surface.director = this;
            surface.eventor = surface.AddComp<Eventor>();
            surface.eventor.Create();
            surfaces.Add(surface);
            surfaceDict.Add(surface.id, surface);
            surface.Create();
        }

        private void OnAddActor(AddActorEvent e)
        {
            Debug.Log($"OnAddActor =====>{e.actor}");
            
            var surface = GetSurface(e.actor);
            if (null == surface) return;
            RmvComp(surface);
            surface.Destroy();
        }
    }
}