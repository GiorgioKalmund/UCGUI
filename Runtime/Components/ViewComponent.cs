using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UCGUI
{
    public class ViewComponent : ImageComponent, IRenderable
    {
        public bool isOpen = false;

        public UnityEvent onPopupOpen;
        public UnityEvent onPopupClose;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        
        public static Color DefaultBackdropColor = UnityEngine.Color.black;
        public static float DefaultBackdropAlpha = 0.6f;

        private InputAction _toggleAction;

        private Button _button;

        public bool closesOnBackgroundTap = false;

        public override void Awake()
        {
            base.Awake();

            onPopupOpen ??= new UnityEvent();
            onPopupClose ??= new UnityEvent();

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            
            _button = gameObject.GetOrAddComponent<Button>();
            _button.transition = Selectable.Transition.None;
            _button.onClick.AddListener(OnBackgroundTap);
        }
    
        public override void Start()
        {
            base.Start();
            
            this.DisplayName("PopupComponent");
            
            this.FullScreen(_canvas);
            
            if (_toggleAction != null)
            {
                _toggleAction.performed += ToggleOpenClose;
                _toggleAction?.Enable();
            }

            Color(DefaultBackdropColor, DefaultBackdropAlpha);
            
            if (isOpen) Open(); else Close();
        }

        public ViewComponent DontCloseOnStart()
        {
            isOpen = true;
            return this;
        }

        public void Render() { }

        public virtual void ToggleOpenClose(InputAction.CallbackContext context)
        {
            if (isOpen)
                Close();
            else
                Open();
        }

        public virtual void Open()
        {
            _canvasGroup.alpha = 1f;

            isOpen = true;
            this.BringToFront();
            Render();

            onPopupOpen?.Invoke();
        }

        public virtual void Close()
        {
            _canvasGroup.alpha = 0f;
            
            isOpen = false;

            onPopupClose?.Invoke();
        }

        public ViewComponent Add(params BaseComponent[] components)
        {
            foreach (var baseComponent in components)
            {
                Add(baseComponent);
            }
            return this;
        }
        public ViewComponent Add(BaseComponent component)
        {
            component.Parent(this);
            return this;
        }
        
        public ViewComponent ToggleUsing(InputAction toggle)
        {
            _toggleAction = toggle;
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

        public ViewComponent Link(Canvas canvas)
        {
            _canvas = canvas;
            return this.Parent(canvas);
        }

        private void OnBackgroundTap()
        {
            if (closesOnBackgroundTap && isOpen)
                Close();
        }

        public class ViewBuilder
        {
            private readonly ViewComponent _view;

            public ViewBuilder(ViewComponent view, Canvas canvas)
            {
                view.Link(canvas);
                _view = view; 
            }

            public void ToggleUsing(InputAction toggle)
            {
                _view.ToggleUsing(toggle);
            }

            public void Add(params BaseComponent[] contents)
            {
                _view.Add(contents);
            }

            public void CloseOnBackgroundTap(bool closes = true)
            {
                _view.closesOnBackgroundTap = closes;
            }
            public void DontCloseOnStart()
            {
                _view.DontCloseOnStart();
            }
        }
    }
}