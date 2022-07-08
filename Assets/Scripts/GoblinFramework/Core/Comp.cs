using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    internal abstract class Comp : Goblin
    {
        private List<Comp> comps = null;

        protected override void OnCreate()
        {
            comps = new List<Comp>();
        }

        protected override void OnDestroy()
        {
            foreach (var comp in comps) RmvComp(comp);
            comps.Clear();
            comps = null;
        }

        internal List<T> GetComp<T>() where T : Comp
        {
            if (null == comps) throw new Exception("comps is null.");

            return null;
        }

        internal T AddComp<T>() where T : Comp, new()
        {
            T comp = new T();
            if (comp is Entity) throw new Exception("plz don't add entity to entity.");
            comp.Create(this);

            return comp;
        }

        internal void RmvComp(Comp comp)
        {
            if (false == comps.Contains(comp)) throw new Exception("donot has comp in this entity.");
            comp.Destroy();
            comps.Remove(comp);
        }

        internal void RmvComp<T>()
        {
            for (int i = comps.Count; i > 0; i--) if (comps[i] is T) RmvComp(comps[i]);
        }
    }
}
