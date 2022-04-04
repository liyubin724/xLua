using System.Collections.Generic;
using UnityEngine;

namespace DotEngine.UI
{
    public class UIRoot : MonoBehaviour
    {
        public static UIRoot Root = null;

        public Camera uiCamera;
        public UILayerCanvas[] layerCanvases = new UILayerCanvas[0];

        private Dictionary<UILayerLevel, UILayerCanvas> layerCanvasDic = new Dictionary<UILayerLevel, UILayerCanvas>();

        private void Awake()
        {
            if(Root!=null && Root!=this)
            {
                Destroy(this);
            }
            else
            {
                Root = this;
                DontDestroyOnLoad(gameObject);

                if(layerCanvases!=null)
                {
                    foreach(var layerCanvas in layerCanvases)
                    {
                        if(layerCanvasDic.ContainsKey(layerCanvas.layerLevel))
                        {
                            Debug.LogError("UIRoot::Awake->The layer has been added");
                        }
                        else
                        {
                            layerCanvasDic.Add(layerCanvas.layerLevel, layerCanvas);
                        }
                    }
                }
            }
        }

        public RectTransform GetLayerTransform(UILayerLevel layerLevel)
        {
            if (layerCanvasDic.TryGetValue(layerLevel, out var layerCanvas))
            {
                return layerCanvas.LayerTransform;
            }
            return null;
        }

        public GameObject GetLayerGameObject(UILayerLevel layerLevel)
        {
            if (layerCanvasDic.TryGetValue(layerLevel, out var layerCanvas))
            {
                return layerCanvas.LayerGameObject;
            }
            return null;
        }

        public void SetLayerVisible(UILayerLevel layerLevel,bool visible)
        {
            if (layerCanvasDic.TryGetValue(layerLevel, out var layerCanvas))
            {
                if(layerCanvas.Visible!=visible)
                {
                    layerCanvas.Visible = visible;
                }
            }
        }

        public bool GetLayerVisible(UILayerLevel layerLevel)
        {
            if (layerCanvasDic.TryGetValue(layerLevel, out var layerCanvas))
            {
                return layerCanvas.Visible;
            }
            return false;
        }
    }
}
