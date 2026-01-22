using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UCGUI.Services
{
    /// <summary>
    /// A simple service for accessing some things relevant to GUIs
    /// </summary>
    public partial class GUIService
    {
        private static Canvas _cachedCanvas;
        
        /// <summary>
        /// Finds the first canvas which is in <see cref="RenderMode.ScreenSpaceOverlay"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Canvas"/> if one was found, else throws an exception.
        /// </returns>
        /// <exception cref="Exception">If no canvas is found.</exception>
        public static Canvas GetCanvas(bool forceNewCache = false)
        {
            if (_cachedCanvas && !forceNewCache)
                return _cachedCanvas;
            var canvasList = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var c in canvasList)
            {
                if (c.renderMode.Equals(RenderMode.ScreenSpaceOverlay))
                {
                    _cachedCanvas = c;
                    break;
                }
            }
            if (!_cachedCanvas)
                throw new Exception("Could not find Canvas!");
            return _cachedCanvas;
        }

        /// <summary>
        /// (Kind of uselss) "shorthand" for `Camera.main`
        /// </summary>
        /// <returns>The main <see cref="Camera"/> if present.</returns>
        public static Camera GetMainCamera()
        {
            return Camera.main;
        }
    
        /// <summary>
        /// Finds a canvas using <see cref="GetCanvas"/>, or uses the cached one, and returns its width.
        /// </summary>
        /// <returns>The width of the canvas if found, else throws null exception.</returns>
        public static float GetCanvasWidth()
        {
            return GetCanvas().GetComponent<RectTransform>().sizeDelta.x;
        }
    
        /// <summary>
        /// Finds a canvas using <see cref="GetCanvas"/>, or uses the cached one, and returns its height.
        /// </summary>
        /// <returns>The height of the canvas if found, else throws null exception.</returns>
        public static float GetCanvasHeight()
        {
            return GetCanvas().GetComponent<RectTransform>().sizeDelta.y;
        }

        /// <summary>
        /// A factor describing the change in width relative to the <see cref="Defaults.Screen.ReferenceResolution"/>
        /// Multiply it with fixed dimensions to scale to a different, non-native, resolution
        /// </summary>
        public static float WidthScale = GetCanvasWidth() / Defaults.Screen.ReferenceResolution.x;
        /// <summary>
        /// A factor describing the change in height relative to the <see cref="Defaults.Screen.ReferenceResolution"/>
        /// Multiply it with fixed dimensions to scale to a different, non-native, resolution
        /// </summary>
        public static float HeightScale = GetCanvasHeight() / Defaults.Screen.ReferenceResolution.y;
    }
}