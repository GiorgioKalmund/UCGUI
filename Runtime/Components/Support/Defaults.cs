using TMPro;
using UnityEngine;
using UCGUI.Services;

namespace UCGUI{
    /// <summary>
    /// Statically stored defaults for UCGUI
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// Holds general state information for UCGUI
        /// </summary>
        public static class State
        {
            
        }
        
        public static class Screen 
        {
            /// <summary>
            /// Reference resolution value. This can be used to dynamically scale your UI based on the real
            /// resolution of the device the program is running on. Used to calculate <see cref="GUIService.WidthScale"/> and  <see cref="GUIService.HeightScale"/>.
            /// </summary>
            public static Vector2 ReferenceResolution = new Vector2(1920, 1080);
        }

        public static class Debug
        {
            public static GUIStyle DebugStyle(Color color, int fontSize = 14, FontStyle fontStyle = FontStyle.Bold)
            {
                var style = new GUIStyle();
                style.normal.textColor = color;
                style.fontStyle = fontStyle;
                style.fontSize = fontSize;
                return style;
            }
            
            public static GUIStyle DebugWhite(int fontSize = 14) => DebugStyle(Color.white, fontSize);
            public static GUIStyle DebugBlack(int fontSize = 14) => DebugStyle(Color.black, fontSize);
            public static GUIStyle DebugRed(int fontSize = 14) => DebugStyle(Color.red, fontSize);
        }

        public static class Text
        {
            /// <summary>
            /// Accessor for <see cref="TextComponent.GlobalFont"/>. The specified font will be used for every instantiated
            /// text. <i>(If not overridden)</i>
            /// </summary>
            public static TMP_FontAsset GlobalFont
            {
                get => TextComponent.GetGlobalFont();
                set => TextComponent.GlobalFont(value);
            }

            /// <summary>
            /// The size a text should be if no size is specified. Defaults to Unity's convention of 100 x 100.
            /// </summary>
            public static Vector2 DefaultSize = new Vector2(100, 100);
        }

        public static class Spacer
        {
            /// <summary>
            /// The default width or height of the spacer into its non-stretching dimension.
            /// </summary>
            public static float AlternateDirectionExtents = 25f;
        }

        /// <summary>
        /// Default options for <see cref="IFocusable"/>.
        /// </summary>
        public static class Focus
        {
            /// <summary>
            /// The default group to use as fallback. Every IFocusable member will automatically be part of this group if
            /// not specified otherwise using <see cref="IFocusable.FocusGroup"/>.
            /// </summary>
            /// <remarks>
            /// "ucgui-focus-group-default"
            /// </remarks>
            public static readonly string DefaultGroup = "ucgui-focus-group-default";
        }

        /// <summary>
        /// Default behaviours and options for the <see cref="AbstractViewComponent"/>.
        /// </summary>
        public static class View
        {
            /// <summary>
            /// Whether the view should start open by default (after creation).
            /// </summary>
            public static bool StartsOpen = false;

            /// <summary>
            /// Default backdrop color of the view.
            /// </summary>
            public static Color DefaultBackdropColor = Color.clear;
        }

        public static class Services
        {
            /// <summary>
            /// Path to a Texture(2D) which should be displayed if a path is unavailable.
            /// Defaults to `Assets/Resources/Textures/{location}(.png)` when using image service direct Sprite access,
            /// otherwise simply `Assets/Resources/{location}(.png)` when using direct Texture2D loading.
            /// </summary>
            public static string MissingTexture2DLocation = "missing";
        }
        
        public static class Image
        {
            /// <summary>
            /// The size an image should be if no size is specified. Defaults to Unity's convention of 100 x 100.
            /// </summary>
            public static Vector2 DefaultSize = new Vector2(100, 100);
        }
    }
}