using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    internal abstract class Comp : Goblin
    {
        private List<Comp> compList = null;
        private Dictionary<string, List<Comp>> compDict = new Dictionary<string, List<Comp>>();

        protected override void OnCreate()
        {
            compList = new List<Comp>();
        }

        protected override void OnDestroy()
        {
            foreach (var comp in compList) RmvComp(comp);
            compList.Clear();
            compList = null;

            compDict.Clear();
            compDict = null;
        }

        internal List<T> GetComp<T>() where T : Comp
        {
            if (null == compList) throw new Exception("comps is null.");

            return null;
        }

        internal T AddComp<T>() where T : Comp, new()
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

        internal void RmvComp(Comp comp)
        {
            comp.Destroy();

            if (compDict.TryGetValue(nameof(comp), out List<Comp> comps)) comps.Remove(comp);
            compList.Remove(comp);
        }

        internal void RmvComp<T>()
        {
            if (compDict.TryGetValue(nameof(T), out List<Comp> comps)) for (int i = comps.Count; i > 0; i--) RmvComp(comps[i]);
        }
    }
}
