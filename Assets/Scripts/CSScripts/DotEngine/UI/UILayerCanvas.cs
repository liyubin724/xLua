using UnityEngine;

namespace DotEngine.UI
{
    public enum UILayerLevel
    {
        BottomlowestLayer = 0,
        BottomLayer,
        DefaultLayer,
        TopLayer,
        TopmostLayer,
    }

    [RequireComponent(typeof(Canvas))]
    public class UILayerCanvas : MonoBehaviour
    {
        public UILayerLevel layerLevel = UILayerLevel.DefaultLayer;
        public RectTransform LayerTransform { get; private set; }
        public GameObject LayerGameObject { get; private set; }

        private bool m_Visible = true;
        public bool Visible
        {
            get
            {
                return m_Visible;
            }
            set
            {
                if (m_Visible != value)
                {
                    m_Visible = value;
                    LayerGameObject.SetActive(m_Visible);
                }
            }
        }

        private void Awake()
        {
            LayerTransform = (RectTransform)transform;
            LayerGameObject = gameObject;
        }
    }
}
