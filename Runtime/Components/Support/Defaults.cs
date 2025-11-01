using TMPro;
using UnityEditor;
using UnityEngine;

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
        /// Default behaviours and options for the <see cref="ViewComponent"/>.
        /// </summary>
        public static class View
        {
            /// <summary>
            /// Automatically adds events <see cref="ViewComponent.OnOpen"/> and <see cref="ViewComponent.OnClose"/> for every View.
            /// </summary>
            public static bool AutoAddEvents = false;

            public static bool StartsOpen = true;
            
            public static Color DefaultBackdropColor = Color.black;
            public static float DefaultBackdropAlpha = 0.6f;
        }

        public static class Services
        {
            public static string TexturesLocation = "Textures/";
            public static string MissingTexture2DLocation = "missing";
        }
    }
}