using System;
using GoblinFramework.Render.Common;
using System.Collections;
using System.Collections.Generic;
using GoblinFramework.Common.Events;
using UnityEngine;

namespace GoblinFramework.Render.Gameplay
{
    public class Surface : RComp
    {
        public uint id;
        public SurfaceDirector director;
        public Eventor eventor;

        private Dictionary<Type, Dressing> dressingDict = new();
        
        public T GetDressing<T>() where T : Dressing
        {
            if (dressingDict.TryGetValue(typeof(T), out var dressing)) return dressing as T;

            return null;
        }
        
        public T AddDressing<T>() where T : Dressing, new()
        {
            if (dressingDict.ContainsKey(typeof(T))) throw new Exception($"can't add same dressing -> {typeof(T)}");

            var dressing = AddComp<T>();
            dressing.surface = this;
            dressingDict.Add(typeof(T), dressing);

            return dressing;
        }

        public void RmvDressing<T>() where T : Dressing
        {
            RmvDressing(GetDressing<T>());
        }

        public void RmvDressing(Dressing dressing)
        {
            dressingDict.Remove(dressing.GetType());
        }
    }
}