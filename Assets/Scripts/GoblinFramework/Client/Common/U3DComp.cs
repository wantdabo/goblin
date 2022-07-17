using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Common
{
    public class U3DComp : Comp
    {
        public T GetNode<T>(GameObject go) where T : UnityEngine.Object
        {
            if (go is T) return go as T;

            return go.GetComponent<T>();
        }

        public T GetNode<T>(GameObject go, string path) where T : UnityEngine.Object
        {
            var node = go.transform.Find(path).gameObject;
            if (null == node) return null;

            if (node is T) return node as T;
            return node.GetComponent<T>();
        }

        public T SeekNode<T>(GameObject go, string name) where T : UnityEngine.Object
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var node = go.transform.GetChild(i).gameObject;
                if (node.name.Equals(name))
                {
                    if (node is T) return node as T;
                    return node.GetComponent<T>();
                }

                var child = SeekNode<T>(node, name);
                if (null == child) continue;

                if (child is T) return child as T;
                return (child as GameObject).GetComponent<T>();
            }

            return null;
        }
    }
}
