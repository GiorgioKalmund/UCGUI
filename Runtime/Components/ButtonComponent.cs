using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCGUI
{
    public class ButtonComponent : ImageComponent, IFocusable, ICopyable<ButtonComponent>
    {
        public ButtonComponent() { NamePrefix = "ButtonComponent"; }
        
        // -- Button -- //
        protected Button ButtonElement;
        public List<UnityAction> Listeners = new List<UnityAction>();

        // -- Subcomponents -- // 
        protected ImageComponent ForegroundImage;
        protected TextComponent ButtonText;
        
        private bool _focusable;
        
        public int FocusGroup { get; protected set; } = 0;
        
        public override void Awake()
        {
            base.Awake();

            ButtonElement = gameObject.GetOrAddComponent<Button>();

            ForegroundImage = ComponentExtension.N<ImageComponent>(transform, "Foreground-Hint")
                    .RaycastTarget(false)
                    .SetActive(false)
                ;

            ButtonText = ComponentExtension.N<TextComponent>(transform, "Text")
                    .AlignCenter()
                    .VAlignCenter()
                    .Color(UnityEngine.Color.gray1)
                ;
        }

        public override void Start()
        {
            base.Start();
            this.SafeDisplayName("ButtonComponent");

            if (_focusable)
                Function(this.Focus);
        }

        public ButtonComponent Create(string text = "", UnityAction action = null, Sprite foreground = null, bool focusable = false)
        {
            Text(text);
            if (action != null)
                Function(action);
            if (foreground)
            {
                Foreground(foreground);
            }

            _focusable = focusable;
            
            return this;
        }
        
        public ButtonComponent Foreground(Sprite sprite, float alpha = 1f)
        {
            ForegroundImage.Sprite(sprite).Alpha(alpha).SetActive(sprite);
            return this;
        }

        public ButtonComponent FitToContents(PaddingSide side, int amount, float spacing, ScrollViewDirection direction = ScrollViewDirection.Both)
        {
            //AddHorizontalLayout(spacing, childControlWidth:true, childControlHeight:true);
            AddFitter(direction);
            ButtonText.FitToContents();
            
            AddHorizontalLayout(spacing);
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

        public ButtonComponent Text(string text, Color? color = null)
        {
            ButtonText.Text(text, color);
            return this;
        }

        public TextComponent GetTextComponent()
        {
            return ButtonText;
        }

        public ButtonComponent Function(UnityAction action, bool keepLast = false)
        {
            if (!keepLast && Listeners.Count > 1)
            {
                var last = Listeners[^1];
                RemoveFunction(last);
            }
            ButtonElement.onClick.AddListener(action);
            Listeners.Add(action);
            return this;
        }
        
        public ButtonComponent RemoveFunction(UnityAction action)
        {
            ButtonElement.onClick.RemoveListener(action);
            Listeners.Remove(action);
            return this;
        }
        
        public ButtonComponent ClearAllFunctions()
        {
            ButtonElement.onClick.RemoveAllListeners();
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
            var colors = ButtonElement.colors;
            colors.highlightedColor = color;
            ButtonElement.colors = colors;
            return this;
        }
        
        public ButtonComponent PressedColor(Color color)
        {
            var colors = ButtonElement.colors;
            colors.pressedColor = color;
            ButtonElement.colors = colors;
            return this;
        }
        
        public ButtonComponent DisabledColor(Color color)
        {
            var colors = ButtonElement.colors;
            colors.disabledColor = color;
            ButtonElement.colors = colors;
            return this;
        }
        
        public ButtonComponent HighlightedSprite(Sprite sprite)
        {
            var sprites = ButtonElement.spriteState;
            sprites.highlightedSprite = sprite;
            ButtonElement.spriteState = sprites;
            return this;
        }
        
        public ButtonComponent PressedSprite(Sprite sprite)
        {
            var sprites = ButtonElement.spriteState;
            sprites.pressedSprite = sprite;
            ButtonElement.spriteState = sprites;
            return this;
        }
        
        public ButtonComponent DisabledSprite(Sprite sprite)
        {
            var sprites = ButtonElement.spriteState;
            sprites.disabledSprite = sprite;
            ButtonElement.spriteState = sprites;
            return this;
        }

        public ButtonComponent Lock()
        {
            ButtonElement.interactable = false;
            return this;
        }

        public ButtonComponent Unlock()
        {
            ButtonElement.interactable = true;
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
            ButtonText.CopyFrom(other.ButtonText);
            ForegroundImage.CopyFrom(other.ForegroundImage);
            Foreground(other.ForegroundImage.GetImage().sprite);
            
            ButtonElement.CopyFrom(other.ButtonElement);
            
            ClearAllFunctions();
            foreach (var unityAction in other.Listeners)
            {
                Function(unityAction);
            }

            return this;
        }

        public virtual void HandleFocus() { }
        public virtual void HandleUnfocus() { }

        public virtual int GetFocusGroup()
        {
            return FocusGroup;
        }

        public virtual void SetFocusGroup(int group)
        {
            FocusGroup = group;
        }

        public ImageComponent GetForeground()
        {
            return ForegroundImage;
        }

        public ButtonComponent Transition(Selectable.Transition transition)
        {
            ButtonElement.transition = transition;
            return this;
        }

        public ButtonComponent SpriteSwap(Sprite highlightedSprite = null, Sprite pressedSprite = null, Sprite disabledSprite = null)
        {
            Transition(Selectable.Transition.SpriteSwap);
            var state = ButtonElement.spriteState;
            HighlightedSprite(highlightedSprite ?? state.selectedSprite);
            PressedSprite(pressedSprite ?? state.selectedSprite);
            DisabledSprite(disabledSprite ?? state.selectedSprite);
            return this;
        }

        public class ButtonBuilder
        {
            private ButtonComponent _button;

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

            public void TextStyle(Color? color = null, FontStyles fontStyle = FontStyles.Normal)
            {
                if (color.HasValue)
                    _button.ButtonText.Color(color.Value);
                _button.ButtonText.FontStyle(fontStyle);
            }
            
            public TextComponent TextBuilder()
            {
                return _button.ButtonText;
            }
        }
    }
}