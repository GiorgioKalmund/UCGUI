using TMPro;
using UnityEditor;
using UnityEngine;

namespace UCGUI{
    /// <summary>
    /// Statically stored defaults for UCGUI
    /// </summary>
    public static class Defaults
    {
        public static class State
        {
            public static bool DebugMode = false;
            #if UNITY_EDITOR
            [MenuItem("Tools/UCGUI/Toggle Debug Mode %#d")]
            private static void ToggleDebugMode()
            {
                DebugMode = !DebugMode;
                Menu.SetChecked("Tools/UCGUI/Toggle Debug Mode %d", DebugMode);
                Debug.LogWarning($"UCGUI: Debug Mode {(DebugMode ? "enabled" : "disabled")}.");
            }
            #endif

            public static GUIStyle DebugStyle
            {
                get
                {
                    var style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.fontStyle = FontStyle.Bold;
                    return style;
                }
            }
            public static GUIStyle DebugStyle2
            {
                get
                {
                    var style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    style.fontStyle = FontStyle.Bold;
                    return style;
                }
            }
        }
        
        public static class Text
        {
            public static TMP_FontAsset GlobalFont
            {
                get => TextComponent.GetGlobalFont();
                set => TextComponent.GlobalFont(value);
            }
        }

        public static class Focus
        {
            public static readonly string DefaultGroup = "ucgui-focus-group-0";
        }

        public static class View
        {
            public static bool AutoAddEvents = false;
            
            public static Color DefaultBackdropColor = Color.black;
            public static float DefaultBackdropAlpha = 0.6f;
        }

        public static class Style
        {
            public static class Button
            {
                public static readonly ButtonStyle Default = new ButtonStyle(btn =>
                {
                    btn.TextBuilder().Style(Text.ButtonText);
                });

                public static readonly ButtonStyle DefaultFit = Default.Expand(btn =>
                {
                    btn.FitToContents(20, 20);
                });
            }

            public static class Text
            {
                public static readonly TextStyle Primary = new TextStyle(txt =>
                {
                    txt.VAlignCenter().Color(Color.gray1);
                });

                public static readonly TextStyle Secondary = Primary.Expand(txt =>
                {
                    txt.Color(Color.gray);
                });
                
                public static readonly TextStyle ButtonText = Primary.Expand(txt =>
                {
                    txt.AlignCenter();
                });
            }

            public static class Input
            {
                public static readonly InputStyle Default = new InputStyle(input =>
                {
                    input.GetTextComponent().Style(Text.Primary);
                    input.GetPlaceholder().Style(Text.Secondary);
                });
            }
        }
    }
}