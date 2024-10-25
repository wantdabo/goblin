using UnityEngine;

namespace ShapeDrawers.Common
{
    public abstract class Drawer
    {
        public static GameObject drawerpool { get; private set; } = default;

        public abstract DrawerType type { get; }

        public GameObject gameObject { get; private set; }

        public Material mat { get; set; }

        public bool enabled
        {
            get
            {
                if (null == gameObject) return false;

                return gameObject.activeSelf;
            }
        }

        public Drawer()
        {
            gameObject = OnGenerate();
            if (null == drawerpool) drawerpool = new("__SHAPE_DRAWER__");
            gameObject.transform.SetParent(drawerpool.transform, false);
            var mr = gameObject.GetComponent<MeshRenderer>();
            mat = new Material(Shader.Find("ShapeDrawer/Drawer"));
            SetColor(Color.magenta);
            mr.material = mat;
        }

        public void SetColor(Color color)
        {
            color.a *= 0.5f;
            mat.SetColor("_Color", color);
        }

        public void Enabled()
        {
            if (null == gameObject) return;
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            if (null == gameObject) return;
            gameObject.SetActive(false);
        }

        protected abstract GameObject OnGenerate();
    }
}
