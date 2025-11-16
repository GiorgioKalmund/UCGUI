using System.Collections.Generic;
using JetBrains.Annotations;
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
    /// <item><description><see cref="FitToContents(int,float,UCGUI.ScrollViewDirection)"/> - Allows the button to fit to its contents, as well as aligning text and foreground image (if present) horizontally. <b>For better control and flexibility use the built-in <see cref="UI.Button"/>.</b></description></item>
    /// </list>
    /// <para>
    /// Also implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    /// </summary>
    public partial class ButtonComponent : LabelComponent, IFocusable, ICopyable<ButtonComponent>, IStylable<ButtonComponent, ButtonStyle>
    {
        public Button button;

        protected readonly List<UnityAction> listeners = new List<UnityAction>();

        public override void Awake()
        {
            base.Awake();
            
            GetGraphic().enabled = true;
            button = gameObject.GetOrAddComponent<Button>();
            Style(ButtonStyle.Plain);
        }

        public override void Start()
        {
            base.Start();
            DisplayName = "Button";
        }

        public ButtonComponent FitToContents(PaddingSide side, int amount, float spacing, ScrollViewDirection direction = ScrollViewDirection.Both)
        {
            AddFitter(direction);
            Padding(side, amount, direction);
            Spacing(spacing);
            return this;
        }
        
        public ButtonComponent FitToContents(int padding = 0, float spacing = 0f, ScrollViewDirection direction = ScrollViewDirection.Both)  
        {
            return FitToContents(PaddingSide.All, padding, spacing, direction);
        }
        

        public ButtonComponent Function(UnityAction action, bool keepLast = false)
        {
            if (!keepLast && listeners.Count > 1)
            {
                var last = listeners[^1];
                RemoveFunction(last);
            }
            button.onClick.AddListener(action);
            listeners.Add(action);
            return this;
        }
        
        public ButtonComponent RemoveFunction(UnityAction action)
        {
            button.onClick.RemoveListener(action);
            listeners.Remove(action);
            return this;
        }
        
        public ButtonComponent ClearAllFunctions()
        {
            button.onClick.RemoveAllListeners();
            listeners.Clear();
            return this;
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);
            if (HorizontalLayout && HorizontalLayout.enabled || VerticalLayout && VerticalLayout.enabled)
                return this;
            
            _image?.Size(x, y);
            _text?.Size(x, y);
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

        public ButtonComponent CopyFrom(ButtonComponent other, bool fullyCopyRect = true)
        {
            if (other.ContentSizeFitter)
                FitToContents();
            
            base.CopyFrom(other, fullyCopyRect);
            button.CopyFrom(other.button);
            
            ClearAllFunctions();
            foreach (var unityAction in other.listeners)
            {
                Function(unityAction);
            }

            return this;
        }


        public string FocusGroup { get; set; }
        
        public UnityEvent OnFocusEvent { get; set; }
        
        public UnityEvent OnUnfocusEvent { get; set; }
        
        public virtual void HandleFocus() { }
        public virtual void HandleUnfocus() { }

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
            style.Apply(this);
            return this;
        }

        public ButtonComponent TargetGraphic(Graphic g)
        {
            button.targetGraphic = g;
            return this;
        }
        
        public override void Enabled(bool on)
        {
            base.Enabled(on);
            button.interactable = on;
            _image?.Enabled(on);
            _text?.Enabled(on);
            if (HorizontalLayout) HorizontalLayout.enabled = on;
            if (VerticalLayout) VerticalLayout.enabled = on;
            if (ContentSizeFitter) ContentSizeFitter.enabled = on;
        }

        #if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            this.DrawFocusableDebug();
        }
        #endif

        public partial class ButtonBuilder
        {
            private ButtonComponent _button;

            public ButtonBuilder(ButtonComponent button) => _button = button;
            
            /// <summary>
            /// Modifies the label of the button.
            /// </summary>
            /// <param name="text">The text of the label.</param>
            /// <param name="image">(Optional) The image of the label.</param>
            public void Label(string text, [CanBeNull] Sprite image = null) => _button.Init(text, image);

            /// <summary>
            /// Hides the default <see cref="LabelComponent"/> elements and adds all inserted elements into the button's body.
            /// </summary>
            /// <param name="content">The content to show.</param>
            public void Content(params BaseComponent[] content) 
            {
                _button.Style(LabelStyle.Hidden);
                _button.Add(content);
            }
        }
    }
}