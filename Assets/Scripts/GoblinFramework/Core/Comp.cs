using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    public abstract class Comp : Goblin
    {
        private List<Comp> compList = new List<Comp>();
        private Dictionary<string, List<Comp>> compDict = new Dictionary<string, List<Comp>>();

        protected override void OnCreate()
        {
        }

        protected override void OnDestroy()
        {
            foreach (var comp in compList) RmvComp(comp);
            compList.Clear();
            compList = null;

            compDict.Clear();
            compDict = null;
        }

        public virtual List<T> GetComp<T>() where T : Comp
        {
            if (compDict.TryGetValue(nameof(T), out List<Comp> comps)) return comps as List<T>;

            return null;
        }

        public virtual T AddComp<T>() where T : Comp, new()
        {
            T comp = new T();

            if (false == compDict.TryGetValue(nameof(comp), out List<Comp> comps))
            {
                comps = new List<Comp>();
                compDict.Add(nameof(comp), comps);
            }
            comps.Add(comp);
            compList.Add(comp);

            comp.Create(this);

            return comp;
        }

        public virtual void RmvComp(Comp comp)
        {
            comp.Destroy();

            if (compDict.TryGetValue(nameof(comp), out List<Comp> comps)) comps.Remove(comp);
            compList.Remove(comp);
        }

        public virtual void RmvComp<T>()
        {
            if (compDict.TryGetValue(nameof(T), out List<Comp> comps)) for (int i = comps.Count; i > 0; i--) RmvComp(comps[i]);
        }
    }
}
