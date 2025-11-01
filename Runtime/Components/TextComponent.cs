using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FontStyles = TMPro.FontStyles;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default Text Component.
    /// <br></br>
    /// <br></br>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="Color"/> - Sets the color.</description></item>
    /// <item><description><see cref="GraphicComponent{T}.Alpha"/> - Sets the alpha.</description></item>
    /// <item><description><see cref="FontStyle"/> - Sets the <see cref="FontStyles"/>. Can be '|' together to layer them. Shorthands: <see cref="Bold"/>,<see cref="Italic"/>,<see cref="Underline"/></description></item>
    /// <item><description><see cref="FontSize"/> and <see cref="AutoSize"/> - Control the text's font size.</description></item>
    /// <item><description><see cref="Alignment"/> - Controls the horizontal alignment. See also <see cref="AlignCenter"/>.</description></item>
    /// <item><description><see cref="VAlignment"/> - Controls the vertical alignment. See also <see cref="VAlignCenter"/>.</description></item>
    /// <item><description><see cref="FitToContents"/> - Makes the rect shrink and expand to it's preferred size.</description></item>
    /// <item><description><see cref="OverflowMode"/> - Controls the overflow behaviour.</description></item>
    /// <item><description><see cref="WrappingMode"/> - Controls the wrapping behaviour. See also <see cref="NoWrap"/></description></item>
    /// </list>
    /// <para>
    /// Also implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    /// </summary>
    public partial class TextComponent : GraphicComponent<TextComponent>, ICopyable<TextComponent>, IStylable<TextComponent, TextStyle>
    {
        private TextMeshProUGUI _textMesh;
        protected static readonly string NamePrefix = "TextComponent";
        private readonly Vector2 _defaultSize = new Vector2(100, 100);
        private static TMP_FontAsset _globalFont;
        protected TextAnimator Animator;

        public static void GlobalFont(TMP_FontAsset asset)
        {
            _globalFont = asset;
        }

        public static TMP_FontAsset GetGlobalFont()
        {
            return _globalFont;
        }
        
        public override void Awake()
        {
            base.Awake();
            _textMesh = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            DisplayName = NamePrefix;
            this.Size(_defaultSize);

            Font(_globalFont);
            Style(TextStyle.Primary);
        }

        public virtual void Start()
        {
            this.SafeDisplayName(NamePrefix);
        }

        public enum TextMode
        {
            Normal, Additive
        }

        public TextComponent Text(string text, TextMode mode = TextMode.Normal, Color? color = null)
        {
            if (text != null)
            {
                if (mode == TextMode.Normal)
                    _textMesh.text = text;
                else if (mode == TextMode.Additive)
                    _textMesh.text += text;
            }
            if (color.HasValue)
                Color(color.Value);
            return this;
        }
        
        public TextComponent Text(int text, TextMode mode = TextMode.Normal)
        {
            return Text(text.ToString(), mode:mode);
        }

        public TextComponent Clear()
        {
            return Text("");
        }

        public string GetText()
        {
            return _textMesh.text;
        }
      
        public TextComponent Font(TMP_FontAsset font)
        {
            _textMesh.font = font;
            return this;
        }
        
        public TextComponent FontSize(float fontSize)
        {
            _textMesh.fontSize = fontSize;
            return this;
        }

        public TextComponent Alignment(TextAlignmentOptions alignmentOptions)
        {
            _textMesh.alignment = alignmentOptions;
            return this;
        }
        
        public TextComponent AlignCenter()
        {
            return Alignment(TextAlignmentOptions.Center);
        }
        
        public TextComponent VAlignment(VerticalAlignmentOptions alignmentOptions)
        {
            _textMesh.verticalAlignment = alignmentOptions;
            return this;
        }
        
        public TextComponent VAlignCenter()
        {
            return VAlignment(VerticalAlignmentOptions.Middle);
        }
        
        public TextComponent OverflowMode(TextOverflowModes overflowModes)
        {
            _textMesh.overflowMode = overflowModes;
            return this;
        }
        
        public TextComponent WrappingMode(TextWrappingModes wrappingModes)
        {
            _textMesh.textWrappingMode = wrappingModes;
            return this;
        }
        
        public TextComponent NoWrap()
        {
            return WrappingMode(TextWrappingModes.NoWrap);
        }

        public TextComponent FontStyle(FontStyles style)
        {
            _textMesh.fontStyle |= style;
            return this;
        }
        public TextComponent FontStyleRemove(FontStyles style)
        {
            _textMesh.fontStyle &= ~style;
            return this;
        }

        public TextComponent FitToContents(bool fit = true, ScrollViewDirection direction = ScrollViewDirection.Horizontal)
        {
            _textMesh.autoSizeTextContainer = fit;
            AddFitter(fit ? direction : ScrollViewDirection.None);
            return this;
        }

        public TextComponent Bold() { return FontStyle(FontStyles.Bold); }
        public TextComponent Italic() { return FontStyle(FontStyles.Italic); }
        public TextComponent Underline() { return FontStyle(FontStyles.Underline); }

        public TextMeshProUGUI GetTextMesh() => _textMesh;
        
        /// <summary>
        /// Return the graphic required by <see cref="GraphicComponent{TextComponent}"/>
        /// </summary>
        public override Graphic GetGraphic() => _textMesh;

        public new TextComponent Copy(bool fullyCopyRect = true)
        {
            TextComponent textCopy = this.BaseCopy(this);
            return textCopy.CopyFrom(this, fullyCopyRect);
        }

        public override TextComponent CopyFrom(TextComponent other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            CopyTextProperties(other.GetTextMesh(), this);
            return this;
        }

        public static void CopyTextProperties(TMP_Text text, TextComponent textComponent)
        {
            textComponent.Text(text.text);
            textComponent.Alignment(text.alignment);
            textComponent.VAlignment(text.verticalAlignment);
            textComponent.FontStyle(text.fontStyle);
            textComponent.FontSize(text.fontSize);
            textComponent.OverflowMode(text.overflowMode);
            textComponent.FitToContents(text.autoSizeTextContainer);
            if (text.enableAutoSizing)
                textComponent.AutoSize(text.fontSizeMin, text.fontSizeMax);
        }

        public TextComponent AutoSize(float minSize = 18, float maxSize = 72, bool active = true)
        {
            _textMesh.enableAutoSizing = active;
            _textMesh.fontSizeMin = minSize;
            _textMesh.fontSizeMax = maxSize;
            return this;
        }

        #if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (debugOptions.HasFlag(DebugOptions.TextOnly))
            {
                Handles.Label(transform.position, $"\"{GetText()}\"\nBold:{(_textMesh.fontStyle & FontStyles.Bold) == FontStyles.Bold}\nItalic:{(_textMesh.fontStyle & FontStyles.Italic) == FontStyles.Italic}\nUnderline:{(_textMesh.fontStyle & FontStyles.Underline) == FontStyles.Underline}", Defaults.Debug.DebugBlack());
                Handles.Label(transform.position + new Vector3(0.2f, 0.2f, 0), $"\"{GetText()}\"\nBold:{(_textMesh.fontStyle & FontStyles.Bold) == FontStyles.Bold}\nItalic:{(_textMesh.fontStyle & FontStyles.Italic) == FontStyles.Italic}\nUnderline:{(_textMesh.fontStyle & FontStyles.Underline) == FontStyles.Underline}", Defaults.Debug.DebugWhite());
            }
        }
        #endif
        
        public TextComponent Style(TextStyle style)
        {
            style.Apply(this);
            return this;
        }
        
        /// <summary>
        /// Creates an <see cref="TextAnimator"/> and adds it to the object. If one is already present, it will 
        /// return the existing.
        /// </summary>
        /// <returns><see cref="Animator"/></returns>
        public TextAnimator AddAnimator()
        {
            Animator = gameObject.GetOrAddComponent<TextAnimator>();
            Animator.DisplayName(DisplayName);
            return Animator;
        }

        public TextComponent Margin(RectOffset rectOffset)
        {
            _textMesh.margin = new Vector4(rectOffset.left, rectOffset.right, rectOffset.top, rectOffset.bottom);
            return this;
        }
    }
}