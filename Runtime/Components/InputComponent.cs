using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default (Text)Input Component.
    /// Default Style: <see cref="InputStyle.Default"/>
    /// </summary>
    public class InputComponent : ImageComponent , IStylable<InputComponent, InputStyle>, IFocusable, ICopyable<InputComponent>
    {
        protected InputComponent() {}
        
        public TMP_InputField input;

        public TextComponent text;
        public TextComponent placeholder;

        protected TMP_InputField.SubmitEvent OnSubmitEvent => input.onSubmit;
        protected TMP_InputField.OnChangeEvent OnChangedEvent => input.onValueChanged;
        protected TMP_InputField.SelectionEvent OnSelectionEvent => input.onSelect;
        protected TMP_InputField.SelectionEvent OnDeselectionEvent => input.onDeselect;

        private int _leadingOffset = 0;
        private int _trailingOffset = 0;

        protected override void Awake()
        {
            base.Awake();
            DisplayName = "Input";

            input = gameObject.GetOrAddComponent<TMP_InputField>();

            text = UI.N<TextComponent>(this).Pivot(PivotPosition.MiddleLeft, true);
            placeholder = UI.N<TextComponent>(this).Pivot(PivotPosition.MiddleLeft, true);

            input.targetGraphic = this.GetImage();
            input.textComponent = text.GetTextMesh();
            input.textViewport = GetRect();
            input.placeholder = placeholder.GetTextMesh();
            input.onFocusSelectAll = true;

            text.OverflowMode(TextOverflowModes.Ellipsis).AutoSize();
            placeholder.OverflowMode(TextOverflowModes.Ellipsis).AutoSize();
            
            OnSelectionEvent.AddListener(_ =>
            {
                this.Focus();
            });
            
            Style(InputStyle.Default);
        }

        public InputComponent Colorize(Color textColor, Color placeholderColor)
        {
            text.Color(textColor, keepPreviousAlphaValue:true);
            placeholder.Color(placeholderColor, keepPreviousAlphaValue:true);
            return this;
        }

        public InputComponent Colorize(Color textColors) => Colorize(textColors, textColors);

        public InputComponent FontSize(float size)
        {
            text.AutoSize(active: false);
            text.FontSize(size);
            placeholder.AutoSize(active: false);
            placeholder.FontSize(size);
            return this;
        }

        public new InputComponent Clear()
        {
            input.text = "";
            return this;
        }

        public InputComponent ContentType(TMP_InputField.ContentType contentType)
        {
            input.contentType = contentType;
            return this;
        }
        public InputComponent InputType(TMP_InputField.InputType inputType)
        {
            input.inputType = inputType;
            return this;
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);

            if (text)
                text.Size(x -_leadingOffset - _trailingOffset, y);
            if (placeholder)
                placeholder.Size(x - _leadingOffset - _trailingOffset, y);
            return this;
        }

        public InputComponent Text(string t)
        {
            input.text = t;
            return this;
        }

        public string GetText()
        {
            return text.GetText();
        }

        public void Select()
        {
            input.ActivateInputField();
        }
        
        public InputComponent Placeholder(string placeholderText)
        {
            this.placeholder.Text(placeholderText);
            return this;
        }

        public InputComponent OnChanged(UnityAction<string> newText)
        {
            OnChangedEvent.AddListener(newText);
            return this;
        }

        public InputComponent OnSubmit(UnityAction<string> submission)
        {
            OnSubmitEvent.AddListener(submission);
            return this;
        }
        
        public InputComponent OnSelect(UnityAction<string> submission)
        {
            OnSelectionEvent.AddListener(submission);
            return this;
        }
        
        public InputComponent OnDeselect(UnityAction<string> submission)
        {
            OnSelectionEvent.AddListener(submission);
            return this;
        }
        
        public InputComponent Style(InputStyle style)
        {
            style.Apply(this);
            return this;
        }
        
        
        public InputComponent PaddingLeading(int offset)
        {
            _leadingOffset = offset;
                
            text.Offset(_leadingOffset, 0);
            placeholder.Offset(_leadingOffset, 0);
            HandleSizeChanged(this.GetWidth(), this.GetHeight());
            return this;
        }
        public InputComponent PaddingTrailing(int offset)
        {
            _trailingOffset = offset;
            HandleSizeChanged(this.GetWidth(), this.GetHeight());
            return this;
        }
        
        public string FocusGroup { get; set; }
        public UnityEvent OnFocusEvent { get; set; }
        public UnityEvent OnUnfocusEvent { get; set; }
        public virtual void HandleFocus() { input.Select(); }
        public virtual void HandleUnfocus() { input.ReleaseSelection(); }
        
        public class InputBuilder
        {
            private InputComponent _input;
            public InputBuilder(InputComponent input) => _input = input;

            public void ContentType(TMP_InputField.ContentType contentType) => _input.ContentType(contentType);
            public void InputType(TMP_InputField.InputType inputType) => _input.InputType(inputType);
            public void PaddingLeading(int offset) => _input.PaddingLeading(offset);
            public void PaddingTrailing(int offset) => _input.PaddingTrailing(offset);
        }

        public new InputComponent Copy(bool fullyCopyRect = true)
        {
            InputComponent copyInput = this.BaseCopy(this);
            return copyInput.CopyFrom(this, fullyCopyRect);
        }

        public InputComponent CopyFrom(InputComponent other, bool fullyCopyRect = true)
        {
            _leadingOffset = other._leadingOffset;
            _trailingOffset = other._trailingOffset;
            base.CopyFrom(other, fullyCopyRect);
            
            text.CopyFrom(other.text, fullyCopyRect);
            placeholder.CopyFrom(other.placeholder, fullyCopyRect);
            
            // TODO: Submit event is not copied over
            return this;
        }
    }
}
