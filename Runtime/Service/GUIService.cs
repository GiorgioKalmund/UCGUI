using System;
using UnityEngine;

namespace UCGUI.Services
{
    public partial class GUIService
    {
        public static Canvas GetCanvas()
        {
            var canvasList = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            Canvas canvas = null;
            foreach (var c in canvasList)
            {
                if (c.renderMode.Equals(RenderMode.ScreenSpaceOverlay))
                {
                    canvas = c;
                    break;
                }
            }
            if (!canvas)
                throw new Exception("Could not find Canvas!");
            return canvas;
        }

    
        public static Camera GetMainCamera()
        {
            return Camera.main;
        }
    
        public static float GetCanvasWidth()
        {
            return GetCanvas().GetComponent<RectTransform>().sizeDelta.x;
        }
    
        public static float GetCanvasHeight()
        {
            return GetCanvas().GetComponent<RectTransform>().sizeDelta.y;
        }

        public static Vector2 GetCanvasSize()
        {
            return new Vector2(GetCanvasWidth(), GetCanvasHeight());
        }

        public static float WidthScale => GetCanvasWidth() / 1170;
        public static float HeightScale => GetCanvasHeight() / 2532;
    }
}