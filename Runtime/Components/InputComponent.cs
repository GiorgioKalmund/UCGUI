using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UCGUI
{
    public partial class InputComponent : BaseComponent, IFocusable
    {
        // -- TMP Input Field -- //
        protected TMP_InputField Input;
        // -- Subcomponents -- //
        protected ImageComponent Backdrop;
        protected TextComponent TextContents;
        protected TextComponent HintContents;

        private InputAction _submitAction;
        public TMP_InputField.SubmitEvent onSubmit => Input.onSubmit;
        public override void Awake()
        {
            base.Awake();
            
            Input = gameObject.GetOrAddComponent<TMP_InputField>();
            
            Backdrop= UI.N<ImageComponent>(transform);

            TextContents = UI.N<TextComponent>(Backdrop);

            HintContents = UI.N<TextComponent>(Backdrop);

            Input.targetGraphic = Backdrop.GetImage();
            Input.textComponent = TextContents.GetTextMesh();
            Input.textViewport = GetRect();
            Input.placeholder = HintContents.GetTextMesh();
            Input.onFocusSelectAll = true;

            TextContents.OverflowMode(TextOverflowModes.Ellipsis);
            HintContents.OverflowMode(TextOverflowModes.Ellipsis);
        }


        public InputComponent Create(string placeholder, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
        {
            ContentType(contentType);
            HintContents.Text(placeholder);
            return this;
        }

        public InputComponent SubmitAction(InputAction action)
        {
            _submitAction = action;
            return this;
        }
        private void OnEnable() { if (_submitAction!= null) _submitAction.performed += SendOnSubmitEvent; }
        private void OnDisable() { if (_submitAction != null) _submitAction.performed -= SendOnSubmitEvent; }
        private void SendOnSubmitEvent(InputAction.CallbackContext context) { Input.OnSubmit(null); }

        public InputComponent Color(Color textColor, Color? hintColor = null)
        {
            TextContents.Color(textColor);
            HintContents.Color(hintColor ?? textColor);
            return this;
        }

        public InputComponent ColorBackdrop(Color color)
        {
            Backdrop.Color(color);
            return this;
        }
        
        public InputComponent FontSize(float size)
        {
            TextContents.FontSize(size);
            HintContents.FontSize(size);
            return this;
        }

        public InputComponent Clear()
        {
            Input.text = "";
            return this;
        }

        public InputComponent ContentType(TMP_InputField.ContentType contentType)
        {
            Input.contentType = contentType;
            return this;
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);

            Backdrop.Size(x, y);
            TextContents.Size(x, y);
            HintContents.Size(x, y);
            return this;
        }

        public TextComponent GetTextComponent() { return TextContents; }
        public string GetText() { return TextContents.GetText(); }
        public TextComponent GetHintComponent() { return HintContents; }

        private void Start()
        {
            DisplayName = "Input";
        }

        public TMP_InputField GetInput()
        {
            return Input;
        }

        public void HandleFocus()
        {
            Input.ActivateInputField();
        }

        public void HandleUnfocus() { }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.Label(transform.position + new Vector3(0, 40, 0), this.IsFocused() + $" ({((IFocusable)this).GetFocusGroup()})");
        }
        #endif
    }
}