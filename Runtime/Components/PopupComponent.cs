using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UCGUI
{
    public class PopupComponent : ButtonComponent, IRenderable
    {
        public bool open = false;

        public UnityEvent onPopupOpen;
        public UnityEvent onPopupClose;

        private Canvas _canvas;
        private Color _defaultBackdropColor = UnityEngine.Color.black;

        private InputAction _toggleAction;

        public override void Awake()
        {
            base.Awake();
            Function(Close);

            onPopupOpen ??= new UnityEvent();
            onPopupClose ??= new UnityEvent();

        }
    
        public override void Start()
        {
            base.Start();
            DisplayName = "PopupComponent";
            
            this.FullScreen(_canvas);
            
            if (_toggleAction != null)
            {
                _toggleAction.performed += ToggleOpenClose;
                _toggleAction?.Enable();
            }
            
            if (open) Open(); else Close();
        }

        protected PopupComponent DontCloseOnStart()
        {
            open = true;
            return this;
        }

        public void Render() { }

        public PopupComponent Build(Canvas canvas, InputAction toggleAction = null, float alpha = 0.6f, Color? backdropColor = null)
        {
            _canvas = canvas;
            _toggleAction = toggleAction;
            
            Color c;
            if (backdropColor.HasValue)
                c = backdropColor.Value;
            else
                c = _defaultBackdropColor;
            
            Color(c);
            DisabledColor(c);
            Alpha(alpha);
            return this;
        }

        public virtual void ToggleOpenClose(InputAction.CallbackContext context)
        {
            if (IsOpen())
                Close();
            else
                Open();
        }

        public virtual void Open()
        {
            GetRect().localScale = Vector2.one;

            open = true;
            this.BringToFront();
            Render();

            onPopupOpen.Invoke();
        }

        public virtual void Close()
        {
            this.LocalScale(Vector2.zero);
            open = false;

            onPopupClose.Invoke();
        }

        public bool IsOpen()
        {
            return open;
        }

        public PopupComponent DontCloseOnBackGroundTap(bool dontClose = true)
        {
            if (dontClose)
                this.Lock();
            else 
                this.Unlock();
            return this;
        }

        public PopupComponent AddContent(BaseComponent component)
        {
            component.Parent(this);
            return this;
        }

        public void OnEnable()
        {
            _toggleAction?.Enable();
        }
        public void OnDisable()
        {
            _toggleAction?.Disable();
        }
    }
}