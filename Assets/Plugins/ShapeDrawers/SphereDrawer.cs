using ShapeDrawers.Common;
using UnityEngine;

namespace ShapeDrawers
{
    public class SphereDrawer : Drawer
    {
        public override DrawerType type => DrawerType.Sphere;
        
        public void Settings(Vector3 position, float radius)
        {
            gameObject.transform.position = position;
            gameObject.transform.localScale = Vector3.one * radius * 2;
        }

        protected override GameObject OnGenerate()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
    }
}
