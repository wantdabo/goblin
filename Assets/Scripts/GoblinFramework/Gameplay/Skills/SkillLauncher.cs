using System;
using System.Collections.Generic;

namespace GoblinFramework.Gameplay.Skills
{
    public class SkillLauncher : Behavior
    {
        private Dictionary<Type, SkillPipeline> skillDict = new Dictionary<Type, SkillPipeline>();

        public SkillPipeline Get<T>()
        {
            return Get(typeof(T));
        }

        public SkillPipeline Get(Type type) 
        {
            if (skillDict.TryGetValue(type, out var skill)) return skill;

            return null;
        }

        public void Load<T>() where T : SkillPipeline, new()
        {
            if (null != Get<T>()) throw new Exception("can't load same skillpipeline in a skilllauncher.");

            var skill = AddComp<T>();
            skill.launcher = this;
            skillDict.Add(typeof(T), skill);
            skill.Create();
        }

        public void Launch<T>() where T : SkillPipeline, new()
        {
            Launch(typeof(T));
        }

        public void Launch(Type type) 
        {
            var skill =  Get(type);
            if (null == skill) return;

            skill.Launch();
        }
    }
}