using ShapeDrawers.Common;
using UnityEngine;

namespace ShapeDrawers
{
    public class CylinderDrawer : Drawer
    {
        public override DrawerType type => DrawerType.Cylinder;

        public void Settings(Vector3 position, float radius, float height, Quaternion rotation)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.transform.localScale = new Vector3(radius * 2, height, radius * 2);
        }

        protected override GameObject OnGenerate()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        }
    }
}
