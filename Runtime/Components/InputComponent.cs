using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default (Text)Input Component.
    /// <i>Extends <see cref="ImageComponent"/>.</i>
    /// <br></br>
    /// <br></br>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="Colorize"/> - Sets the text colors of the text as well as the placeholder.</description></item>
    /// <item><description><see cref="FontSize"/> - Control the text's and placeholder's font size.</description></item>
    /// <item><description><see cref="Clear"/> - Clears the input field.</description></item>
    /// <item><description><see cref="GetText"/> - Returns the current text contents of the input field.</description></item>
    /// </list>
    /// <para>
    /// Handles:
    /// <list type="bullet">
    /// <item><description><see cref="OnSubmit"/> - Add handles to when the user submits their current text input.</description></item>
    /// <item><description><see cref="OnChanged"/> - Add handles to when input's contents change.</description></item>
    /// </list>
    /// Default Style: <see cref="Defaults.Style.Input.Default"/>
    /// </para>
    /// </summary>
    public partial class InputComponent : ImageComponent , IStylable<InputComponent, InputStyle>, IFocusable, ICopyable<InputComponent>
    {
        protected InputComponent() {}
        
        protected TMP_InputField Input;

        protected TextComponent TextElement;
        protected TextComponent PlaceholderElement;

        protected TMP_InputField.SubmitEvent OnSubmitEvent => Input.onSubmit;
        protected TMP_InputField.OnChangeEvent OnChangedEvent => Input.onValueChanged;
        protected TMP_InputField.SelectionEvent OnSelectionEvent => Input.onSelect;
        protected TMP_InputField.SelectionEvent OnDeselectionEvent => Input.onDeselect;

        private int _leadingOffset = 0;
        private int _trailingOffset = 0;

        public override void Awake()
        {
            base.Awake();

            Input = gameObject.GetOrAddComponent<TMP_InputField>();

            TextElement = UI.N<TextComponent>(this).Pivot(PivotPosition.MiddleLeft, true);
            PlaceholderElement = UI.N<TextComponent>(this).Pivot(PivotPosition.MiddleLeft, true);

            Input.targetGraphic = this.GetImage();
            Input.textComponent = TextElement.GetTextMesh();
            Input.textViewport = GetRect();
            Input.placeholder = PlaceholderElement.GetTextMesh();
            Input.onFocusSelectAll = true;

            TextElement.OverflowMode(TextOverflowModes.Ellipsis).AutoSize();
            PlaceholderElement.OverflowMode(TextOverflowModes.Ellipsis).AutoSize();
            
            OnSelectionEvent.AddListener(_ =>
            {
                this.Focus();
            });
            
            Style(InputStyle.Default);
        }

        public InputComponent Colorize(Color textColor, Color placeholderColor)
        {
            TextElement.Color(textColor, keepPreviousAlphaValue:true);
            PlaceholderElement.Color(placeholderColor, keepPreviousAlphaValue:true);
            return this;
        }

        public InputComponent Colorize(Color textColors) => Colorize(textColors, textColors);

        public InputComponent FontSize(float size)
        {
            TextElement.AutoSize(active: false);
            TextElement.FontSize(size);
            PlaceholderElement.AutoSize(active: false);
            PlaceholderElement.FontSize(size);
            return this;
        }

        public new InputComponent Clear()
        {
            Input.text = "";
            return this;
        }

        public InputComponent ContentType(TMP_InputField.ContentType contentType)
        {
            Input.contentType = contentType;
            return this;
        }
        public InputComponent InputType(TMP_InputField.InputType inputType)
        {
            Input.inputType = inputType;
            return this;
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);

            if (TextElement)
                TextElement.Size(x -_leadingOffset - _trailingOffset, y);
            if (PlaceholderElement)
                PlaceholderElement.Size(x - _leadingOffset - _trailingOffset, y);
            return this;
        }

        public TextComponent GetTextComponent()
        {
            return TextElement;
        }

        public string GetText()
        {
            return TextElement.GetText();
        }

        public TextComponent GetPlaceholderComponent()
        {
            return PlaceholderElement;
        }

        public override void Start()
        {
            base.Start();
            
            DisplayName = "Input";
        }

        public TMP_InputField GetInput()
        {
            return Input;
        }

        public void Select()
        {
            Input.ActivateInputField();
        }
        
        public InputComponent Placeholder(string placeholder)
        {
            PlaceholderElement.Text(placeholder);
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
                
            TextElement.Offset(_leadingOffset, 0);
            PlaceholderElement.Offset(_leadingOffset, 0);
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
        public virtual void HandleFocus() { Input.Select(); }
        public virtual void HandleUnfocus() { Input.ReleaseSelection(); }
        
        public class InputBuilder
        {
            private InputComponent _input;
            public InputBuilder(InputComponent input) => _input = input;

            public void ContentType(TMP_InputField.ContentType contentType) => _input.ContentType(contentType);
            public void InputType(TMP_InputField.InputType inputType) => _input.InputType(inputType);
            public void PaddingLeading(int offset) => _input.PaddingLeading(offset);
            public void PaddingTrailing(int offset) => _input.PaddingTrailing(offset);
            public TextComponent GetTextComponent() => _input.GetTextComponent();
            public TextComponent GetPlaceholderComponent() => _input.GetPlaceholderComponent();
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
            
            TextElement.CopyFrom(other.TextElement, fullyCopyRect);
            PlaceholderElement.CopyFrom(other.PlaceholderElement, fullyCopyRect);
            
            // TODO: Submit event is not copied over
            return this;
        }
    }
}
