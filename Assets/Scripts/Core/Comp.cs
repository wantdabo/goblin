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
            foreach (var comp in comps) { RmvComp(comp); }
            comps.Clear();
            comps = null;
        }

        internal List<T> GetComp<T>() where T : Comp
        {
            return null;
        }

        internal T AddComp<T>() where T : Comp, new()
        {
            T comp = new T();
            comp.Create(this);

            return comp;
        }

        internal void RmvComp(Comp comp)
        {
            if (false == comps.Contains(comp)) throw new Exception("remove target comp donot in comps. plz check.");
            comp.Destroy();
            comps.Remove(comp);
        }

        internal void RmvComp<T>()
        {
            for (int i = comps.Count; i > 0; i--)
            {
                if (comps[i] is T) { RmvComp(comps[i]); }
            }
        }
    }
}
