using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common
{
    internal abstract class Comp : Goblin
    {
        public List<Comp> comps = null;

        public override void OnCreate()
        {
            comps = new List<Comp>();
        }

        public override void OnDestroy()
        {
            foreach (var comp in comps) { RmvComp(comp); }
            comps.Clear();
            comps = null;
        }

        public List<T> GetComp<T>() where T : Comp
        {
            return null;
        }

        public T AddComp<T>() where T : Comp, new()
        {
            T comp = new T();
            comp.Create();

            return comp;
        }

        public void RmvComp(Comp comp)
        {
            if (false == comps.Contains(comp)) throw new Exception("remove target comp donot in comps. plz check.");
            comp.Destroy();
            comps.Remove(comp);
        }

        public void RmvComp<T>()
        {
            for (int i = comps.Count; i > 0; i--)
            {
                if (comps[i] is T) { RmvComp(comps[i]); }
            }
        }
    }
}
