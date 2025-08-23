using System;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCGUI
{
    public class UI
    {
        /// <summary>
        /// UCGUI's default Text Component.
        /// </summary>
        /// <param name="text">
        /// The text to display.
        /// </param>
        /// <param name="color">
        /// Optional <see cref="Color"/> of the font. Defaults to TextMeshPro's default.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="TextComponent"/>. Use this to then continue building your desired Text Component.
        /// </returns>
        public static TextComponent Text(string text, Color? color = null)
        {
            TextComponent textComponent = ComponentExtension.N<TextComponent>();
            textComponent.Text(text);
            if (color.HasValue)
                textComponent.Color(color.Value);
            return textComponent;
        }

        /// <summary>
        /// UCGUI's default Image Component.
        /// </summary>
        /// <param name="sprite">
        /// The sprite to display.
        /// </param>
        /// <param name="type">
        /// Optional <see cref="Image.Type"/> used to render the image. Defaults to <see cref="Image.Type.Simple"/>.
        /// </param>
        /// <param name="ppum">
        /// Optional value to determine the PixelsPerUnitMultiplier of the sprite. Relevant for example if you use <see cref="Image.Type.Sliced"/>.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="ImageComponent"/>. Use this to then continue building your desired Image Component.
        /// </returns>
        public static ImageComponent Image(Sprite sprite, Image.Type type = UnityEngine.UI.Image.Type.Simple,
            float ppum = 1f)
        {
            ImageComponent imageComponent = ComponentExtension.N<ImageComponent>();
            imageComponent.Sprite(sprite, type, ppum);
            return imageComponent;
        }

        /// <summary>
        /// UCGUI's default Button Component. Similar to Unity's UGUI's button, however more flexible out of the box.
        /// </summary>
        /// <param name="text">
        /// (Optional) text to be displayed on the button.
        /// </param>
        /// <param name="action">
        /// The action being executed when pressing the button.
        /// </param>
        /// <param name="label">
        /// Label <see cref="ButtonComponent.ButtonBuilder"/> to configure the visuals and formatting behaviour.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="ButtonComponent"/>.
        /// </returns>
        public static ButtonComponent Button([CanBeNull] string text = null, UnityAction action = null,
            UnityAction<ButtonComponent.ButtonBuilder> label = null)
        {
            ButtonComponent buttonComponent = ComponentExtension.N<ButtonComponent>();

            buttonComponent.Text(text);
            if (action != null)
                buttonComponent.Function(action);
            if (label != null)
            {
                label(new ButtonComponent.ButtonBuilder(buttonComponent));
            }

            return buttonComponent;
        }

        /// <summary>
        /// UCGUI's default Window Component.
        /// </summary>
        /// <param name="name">
        /// The name of the window. Will also automatically make this the window's title.
        /// </param>
        /// <param name="builder">
        /// <see cref="WindowComponent.WindowBuilder"/> with most of the commonly used functions used to configure your window.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="WindowComponent"/>.
        /// </returns>
        /// <example>
        /// <code>
        /// 
        /// UCGUI.Button("Press me!", () =>
        /// {
        ///     Debug.Log("Button pressed!");
        /// }, label =>
        /// {
        ///     label.TextStyle(Color.white, FontStyles.Bold | FontStyles.Italic);
        ///     label.FitToContents(spacing:20);
        ///     label.Padding(new RectOffset(20, 20, 20, 20));
        ///     label.Foreground(foreground_image);
        ///     label.Background(Color.gray);
        /// }).Parent(canvas);
        /// </code>
        /// </example>
        public static WindowComponent Window(string name, Action<WindowComponent.WindowBuilder> builder = null)
        {
            WindowComponent window = ComponentExtension.N<WindowComponent>(name);

            if (builder != null)
            {
                var build = new WindowComponent.WindowBuilder(window);
                build.VerticalLayout(10, new RectOffset(10, 10, 10, 10));
                build.FitToContents(ScrollViewDirection.Both);
                build.Header(name, Color.gray8);
                builder(build);
            }

            return window;
        }

        /// <summary>
        /// UCGUI's default Slider Component. Emulates Unity's UGUI native slider element.
        /// </summary>
        /// <param name="range">
        /// The <see cref="Range"/> the of values the slider covers, inclusive. Min has to be larger than Max, otherwise they will be automatically flipped.
        /// </param>
        /// <param name="builder">
        /// <see cref="SliderComponent.SliderBuilder"/> with most of the commonly used functions used to configure your slider.
        /// </param>
        /// <param name="onValueChanged">
        /// Callback invoked every time the slider's value changes.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="SliderComponent"/>
        /// </returns>
        /// <example>
        /// <code>
        /// UCGUI.Slider(new Range(0, 1), builder =>
        /// {
        ///    builder.Foreground(healthbarFull);
        ///    builder.Background(healthbarEmpty);
        ///    builder.Handle(handleSprite);
        ///        
        /// },  newValue =>
        /// { 
        ///     Debug.Log("Slider Value Changed! " + newValue);
        /// }).Parent(canvas);
        /// </code>
        /// </example>.
        public static SliderComponent Slider(Range range, Action<SliderComponent.SliderBuilder> builder,
            Action<float> onValueChanged = null)
        {
            SliderComponent slider = ComponentExtension.N<SliderComponent>();
            if (!range.IsOrdered)
            {
                Debug.LogWarning("Slider range " + range + " was not ordered. Changed to: " + range.Flipped());
                range.Flip();
            }

            slider.MinValue = range.minValue;
            slider.MaxValue = range.maxValue;

            var build = new SliderComponent.SliderBuilder(slider);
            build.HandleWidth(SliderComponent.DefaultSize.y);
            builder(build);

            if (onValueChanged != null)
            {
                slider.OnValueChanged.AddListener(newVal => onValueChanged(newVal));
            }

            return slider;
        }

        /// <summary>
        /// A horizontal layout element, automatically resizing to its contents.
        /// </summary>
        /// <param name="spacing">
        /// Horizontal spacing between the elements.
        /// </param>
        /// <param name="alignment">
        /// Alignment of HStack children.
        /// </param>
        /// <param name="contents">
        /// <see cref="LayoutBuilder"/> to configure spacing and adding contents. 
        /// </param>
        /// <returns>
        /// The resulting HStack.
        /// <remarks>
        /// Under the hood it is simply an <see cref="ImageComponent"/>, allowing you to enable (<see cref="ImageComponent.ToggleVisibility()"/>), and then also directly modify the backdrop.
        /// </remarks>
        /// </returns>
        public static ImageComponent HStack(float spacing, TextAnchor alignment, Action<LayoutBuilder> contents)
        {
            ImageComponent layout = ComponentExtension.N<ImageComponent>();
            layout.AddHorizontalLayout(spacing, alignment);
            layout.AddFitter(ScrollViewDirection.Both);
            layout.ToggleVisibility();
            contents(new LayoutBuilder(layout.HorizontalLayout));
            return layout;
        }
        /// <inheritdoc cref="HStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static ImageComponent HStack(Action<LayoutBuilder> contents)
            => HStack(0f, TextAnchor.MiddleCenter, contents);
        /// <inheritdoc cref="HStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static ImageComponent HStack(float spacing, Action<LayoutBuilder> contents)
            => HStack(spacing, TextAnchor.MiddleCenter, contents);
        /// <inheritdoc cref="HStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static ImageComponent HStack(TextAnchor alignment, Action<LayoutBuilder> contents)
            => HStack(0f, alignment, contents);
        
        /// <summary>
        /// A vertical layout element, automatically resizing to its contents.
        /// </summary>
        /// <param name="spacing">
        /// Vertical spacing between the elements.
        /// </param>
        /// <param name="alignment">
        /// Alignment of VStack children.
        /// </param>
        /// <param name="contents">
        /// <see cref="LayoutBuilder"/> to configure spacing and adding contents. 
        /// </param>
        /// <returns>
        /// The resulting VStack.
        /// <remarks>
        /// Under the hood it is simply an <see cref="ImageComponent"/>, allowing you to enable (<see cref="ImageComponent.ToggleVisibility()"/>), and then also directly modify the backdrop.
        /// </remarks>
        /// </returns>
        public static ImageComponent VStack(float spacing, TextAnchor alignment, Action<LayoutBuilder> contents)
        {
            ImageComponent layout = ComponentExtension.N<ImageComponent>().Sprite(null);
            layout.AddVerticalLayout(spacing, alignment);
            layout.AddFitter(ScrollViewDirection.Both);
            layout.ToggleVisibility();
            contents(new LayoutBuilder(layout.VerticalLayout));
            return layout;
        }
        /// <inheritdoc cref="VStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static ImageComponent VStack(Action<LayoutBuilder> contents)
            => VStack(0f, TextAnchor.MiddleCenter, contents);
        /// <inheritdoc cref="VStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static ImageComponent VStack(float spacing, Action<LayoutBuilder> contents)
            => VStack(spacing, TextAnchor.MiddleCenter, contents);
        /// <inheritdoc cref="VStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static ImageComponent VStack(TextAnchor alignment, Action<LayoutBuilder> contents)
            => VStack(0f, alignment, contents);

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
            public static class TextDefaults
            {
                public static TMP_FontAsset GlobalFont
                {
                    get => TextComponent.GetGlobalFont();
                    set => TextComponent.GlobalFont(value);
                }
            }
        }
    }
}