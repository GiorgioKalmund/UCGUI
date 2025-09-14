using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default Button Component.
    /// Noteworthy formatting functions:
    /// <list type="bullet">
    /// <item><description><see cref="Foreground"/> - Adds a secondary image sitting between text and background. Hidden by default.</description></item>
    /// <item><description><see cref="Function"/> - Adds a listener function for when the button is clicked.</description></item>
    /// <item><description><see cref="Create"/> - Simple builder to create configure your button. <b>For better control and flexibility use the built-in <see cref="UI.Button"/>.</b></description></item>
    /// <item><description><see cref="FitToContents(int,float,UCGUI.ScrollViewDirection)"/> - Allows the button to fit to its contents, as well as aligning text and foreground image (if present) horizontally. <b>For better control and flexibility use the built-in <see cref="UI.Button"/>.</b></description></item>
    /// </list>
    /// <para>
    /// Also implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    /// </summary>
    public partial class ButtonComponent : ImageComponent, IFocusable, ICopyable<ButtonComponent>, IStylable<ButtonComponent, ButtonStyle>, IEnabled
    {
        public ButtonComponent() { NamePrefix = "ButtonComponent"; }
        
        // -- Button -- //
        public Button button;
        public List<UnityAction> Listeners = new List<UnityAction>();

        // -- Subcomponents -- // 
        protected ImageComponent ForegroundImage;
        protected TextComponent ButtonText;
        
        private bool _focusable;
        
        public override void Awake()
        {
            base.Awake();

            button = gameObject.GetOrAddComponent<Button>();
        }

        public override void Start()
        {
            base.Start();
            this.SafeDisplayName(NamePrefix);

            if (_focusable)
                Function(this.Focus);
        }

        public void EnsureTextExists()
        {
            if (!ButtonText)
                ButtonText = UI.N<TextComponent>(transform, "Text").Style(Defaults.Style.Text.ButtonText);
        }

        public void EnsureForegroundExists()
        {
            if (!ForegroundImage)
                ForegroundImage = UI.N<ImageComponent>(transform, "Foreground-Hint").RaycastTarget(false);
        }

        public ButtonComponent Create(string text = "", UnityAction action = null, Sprite foreground = null, bool focusable = false)
        {
            if (!string.IsNullOrEmpty(text))
                Text(text);
            if (action != null)
                Function(action);
            if (foreground)
                Foreground(foreground);

            _focusable = focusable;
            
            return this;
        }
        
        public ButtonComponent Foreground(Sprite sprite, float alpha = 1f)
        {
            EnsureForegroundExists();
            ForegroundImage.Sprite(sprite).Alpha(alpha).SetActive(sprite);
            return this;
        }

        public ButtonComponent FitToContents(PaddingSide side, int amount, float spacing, ScrollViewDirection direction = ScrollViewDirection.Both)
        {
            AddFitter(direction);
            ButtonText?.FitToContents();
            
            AddHorizontalLayout(spacing, childControlWidth:true, childControlHeight:true);
            Padding(side, amount, ScrollViewDirection.Horizontal);
            return this;
        }
        
        public ButtonComponent FitToContents(int padding = 0, float spacing = 0f, ScrollViewDirection direction = ScrollViewDirection.Both)  
        {
            return FitToContents(PaddingSide.All, padding, spacing, direction);
        }
        
        public BaseComponent ContentPadding(int amount)
        {
            return Padding(PaddingSide.All, amount, ScrollViewDirection.Horizontal);
        }
        
        public ButtonComponent ForegroundSize(float x, float y)
        {
            ForegroundImage.AddLayoutElement(x, y);
            return this;
        }

        public ButtonComponent ForegroundSize(Vector2 size)
        {
            return ForegroundSize(size.x, size.y);
        }

        public ButtonComponent ContentSpacing(float spacing)
        {
            if (HorizontalLayout)
                HorizontalLayout.spacing = spacing;
            if (VerticalLayout)
                VerticalLayout.spacing = spacing;
            return this;
        }

        public ButtonComponent Text(string text, TextComponent.TextMode mode = TextComponent.TextMode.Normal, Color? color = null)
        {
            EnsureTextExists();
            ButtonText.Text(text, mode, color);
            return this;
        }

        public TextComponent TextBuilder()
        {
            EnsureTextExists();
            return ButtonText;
        }

        public ButtonComponent Function(UnityAction action, bool keepLast = false)
        {
            if (!keepLast && Listeners.Count > 1)
            {
                var last = Listeners[^1];
                RemoveFunction(last);
            }
            button.onClick.AddListener(action);
            Listeners.Add(action);
            return this;
        }
        
        public ButtonComponent RemoveFunction(UnityAction action)
        {
            button.onClick.RemoveListener(action);
            Listeners.Remove(action);
            return this;
        }
        
        public ButtonComponent ClearAllFunctions()
        {
            button.onClick.RemoveAllListeners();
            Listeners.Clear();
            return this;
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);
            if (HorizontalLayout && HorizontalLayout.enabled || VerticalLayout && VerticalLayout.enabled)
                return this;
            if (ForegroundImage)
                ForegroundImage.Size(x, y);
            if (ButtonText)
                ButtonText.Size(x, y);
            return this;
        }

        public ButtonComponent HighlightedColor(Color color)
        {
            var colors = button.colors;
            colors.highlightedColor = color;
            button.colors = colors;
            return this;
        }
        
        public ButtonComponent PressedColor(Color color)
        {
            var colors = button.colors;
            colors.pressedColor = color;
            button.colors = colors;
            return this;
        }
        
        public ButtonComponent DisabledColor(Color color)
        {
            var colors = button.colors;
            colors.disabledColor = color;
            button.colors = colors;
            return this;
        }
        
        public ButtonComponent HighlightedSprite(Sprite sprite)
        {
            var sprites = button.spriteState;
            sprites.highlightedSprite = sprite;
            button.spriteState = sprites;
            return this;
        }
        
        public ButtonComponent PressedSprite(Sprite sprite)
        {
            var sprites = button.spriteState;
            sprites.pressedSprite = sprite;
            button.spriteState = sprites;
            return this;
        }
        
        public ButtonComponent DisabledSprite(Sprite sprite)
        {
            var sprites = button.spriteState;
            sprites.disabledSprite = sprite;
            button.spriteState = sprites;
            return this;
        }

        public ButtonComponent Lock()
        {
            button.interactable = false;
            return this;
        }

        public ButtonComponent Unlock()
        {
            button.interactable = true;
            return this;
        }

        public ButtonComponent Interactable(bool i)
        {
            button.interactable = i;
            return this;
        }

        public new ButtonComponent Copy(bool fullyCopyRect = true)
        {
            ButtonComponent copyButton = this.BaseCopy(this);
            return copyButton.CopyFrom(this, fullyCopyRect);
        }

        public bool IsFocusable()
        {
            return _focusable;
        }

        public ButtonComponent CopyFrom(ButtonComponent other, bool fullyCopyRect = true)
        {
            if (other.ContentSizeFitter)
                FitToContents();
            
            base.CopyFrom(other, fullyCopyRect);
            Create(focusable:other.IsFocusable());
            if (ButtonText)
                ButtonText.CopyFrom(other.ButtonText);
            if (ForegroundImage)
            {
                ForegroundImage.CopyFrom(other.ForegroundImage);
                Foreground(other.ForegroundImage.GetImage().sprite);
            }
            button.CopyFrom(other.button);
            
            ClearAllFunctions();
            foreach (var unityAction in other.Listeners)
            {
                Function(unityAction);
            }

            return this;
        }

        public virtual void HandleFocus() { }
        public virtual void HandleUnfocus() { }

        public ImageComponent GetForeground()
        {
            return ForegroundImage;
        }

        public ButtonComponent Transition(Selectable.Transition transition)
        {
            button.transition = transition;
            return this;
        }

        public ButtonComponent SpriteSwap(Sprite highlightedSprite = null, Sprite pressedSprite = null, Sprite disabledSprite = null)
        {
            Transition(Selectable.Transition.SpriteSwap);
            var state = button.spriteState;
            HighlightedSprite(highlightedSprite ?? state.selectedSprite);
            PressedSprite(pressedSprite ?? state.selectedSprite);
            DisabledSprite(disabledSprite ?? state.selectedSprite);
            return this;
        }

        public ButtonComponent Style(ButtonStyle style)
        {
            style.Link(this);
            return this;
        }
        
        
        public void ReverseArrangement(ScrollViewDirection affectedDirectionLayout)
        {
            if (affectedDirectionLayout.HasFlag(ScrollViewDirection.Vertical) && VerticalLayout)
                VerticalLayout.ReverseArrangement();
            if (affectedDirectionLayout.HasFlag(ScrollViewDirection.Horizontal) && HorizontalLayout)
                HorizontalLayout.ReverseArrangement();
        }

        public override void Enabled(bool on)
        {
            base.Enabled(on);
            button.interactable = on;
            ForegroundImage?.Enabled(on);
            ButtonText.Enabled(on);
            if (HorizontalLayout) HorizontalLayout.enabled = on;
            if (VerticalLayout) VerticalLayout.enabled = on;
            if (ContentSizeFitter) ContentSizeFitter.enabled = on;
        }

        public partial class ButtonBuilder
        {
            private ButtonComponent _button;

            public HorizontalLayoutGroup HorizontalLayout => _button.HorizontalLayout;
            public VerticalLayoutGroup VerticalLayout => _button.VerticalLayout;

            public ButtonBuilder(ButtonComponent button) { _button = button; }

            public void Foreground(Sprite sprite, float alpha = 1)
            {
                _button.Foreground(sprite, alpha);
            }
            public void Foreground(Sprite sprite, Vector2 preferredSize, float alpha = 1)
            {
                Foreground(sprite, alpha);
                _button.ForegroundImage.Size(preferredSize);
            }
            
            
            public void Background(Sprite sprite, Image.Type type, float ppum = 1f)
            {
                _button.Sprite(sprite, type, ppum);
            }
            public void Background(Color color, float alpha = 1f)
            {
                _button.Color(color, alpha);
            }

            public void AddHorizontalLayout(float spacing = 0f, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false)
            {
                _button.AddHorizontalLayout(spacing, childAlignment, childControlWidth, childControlHeight,
                    childForceExpandWidth, childForceExpandHeight, reverseArrangement);
            }
            public void AddVerticalLayout(float spacing = 0f, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false)
            {
                _button.AddVerticalLayout(spacing, childAlignment, childControlWidth, childControlHeight,
                    childForceExpandWidth, childForceExpandHeight, reverseArrangement);
            }
            
            public void FitToContents(float spacing = 0f, ScrollViewDirection direction = ScrollViewDirection.Both)  
            {
                _button.FitToContents(spacing:spacing, direction:direction);
            }

            public void Padding(RectOffset padding)
            {
                ScrollViewDirection directionToApply = ScrollViewDirection.None;
                if (_button.VerticalLayout)
                    directionToApply |= ScrollViewDirection.Vertical;
                if (_button.HorizontalLayout)
                    directionToApply |= ScrollViewDirection.Horizontal;
                
                Padding(padding, directionToApply);
            }
            public void Padding(RectOffset padding, ScrollViewDirection affectedDirectionLayout)
            {
                _button.Padding(padding, affectedDirectionLayout);
            }

            public void FontStyle(Color? color = null, FontStyles fontStyle = FontStyles.Normal)
            {
                if (color.HasValue)
                    _button.ButtonText.Color(color.Value);
                _button.ButtonText.FontStyle(fontStyle);
            }
            
            public TextComponent TextBuilder()
            {
                return _button.TextBuilder();
            }

            public void ReverseArrangement(ScrollViewDirection affectedDirectionLayout)
            {
                _button.ReverseArrangement(affectedDirectionLayout);
            }
        }
    }
}