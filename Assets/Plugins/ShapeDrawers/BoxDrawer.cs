using ShapeDrawers.Common;
using UnityEngine;

namespace ShapeDrawers
{
    public class BoxDrawer : Drawer
    {
        public override DrawerType type => DrawerType.Box;
        
        public void Settings(Vector3 position, Quaternion rotation, Vector3 size)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.transform.localScale = size;
        }

        protected override GameObject OnGenerate()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }
}
