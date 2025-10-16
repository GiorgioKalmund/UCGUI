using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UCGUI
{
    /// UCGUI's Image Component.
    /// <br></br>
    /// <br></br>
    /// Will default to <see cref="Close"/> on <see cref="Start"/>. Applies <see cref="DefaultBackdropColor"/> and <see cref="DefaultBackdropAlpha"/> to the background by default.
    /// <br></br>
    /// <br></br>
    /// Variables:
    /// <list type="bullet">
    /// <item><description><see cref="isOpen"/> - Whether the view is open.</description></item>.
    /// <item><description><see cref="isLocked"/> - Whether the view is locked If <i>true</i>, the view can neither be opened nor closed.</description></item>.
    /// <item><description><see cref="closesOnBackgroundTap"/> - Determines whether the view should close when clicking on the background.</description></item>.
    /// </list>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="Open"/> - Opens the view.</description></item>.
    /// <item><description><see cref="Close"/> - Closes the view.</description></item>.
    /// <item><description><see cref="ToggleOpenClose()"/> - Toggles between opening and closing the view.</description></item>.
    /// <item><description><see cref="StartOpen"/> - Will <b>not</b> close on <see cref="Start"/>.</description></item>.
    /// <item><description><see cref="OpenUsing"/> - Allows the user to supply and <see cref="InputAction"/> to toggle the opening of the view.</description></item>.
    /// <item><description><see cref="CloseUsing"/> - Allows the user to supply and <see cref="InputAction"/> to toggle the closing of the view.</description></item>.
    /// <item><description><see cref="ToggleUsing"/> - Allows the user to supply and <see cref="InputAction"/> to toggle the opening and closing of the view.</description></item>.
    /// <item><description><see cref="Lock"/> - Locks opening and closing behaviour.</description></item>.
    /// <item><description><see cref="Unlock"/> - Unlocks opening and closing behaviour.</description></item>.
    /// <item><description><see cref="ToggleLock"/> - Toggles between <see cref="Lock"/> and <see cref="Unlock"/>.</description></item>.
    /// </list>
    /// Events:
    /// <list type="bullet">
    /// <item><description><see cref="onOpen"/> - Invoked when the view is opened and either <see cref="EventOpen"/> or <see cref="Events"/> has been called.</description></item>.
    /// <item><description><see cref="onClose"/> - Invoked when the view is closed and either <see cref="EventClose"/> or <see cref="Events"/> has been called.</description></item>.
    /// </list>
    /// <para>
    /// Implements <see cref="IRenderable"/>.
    /// </para>
    public partial class ViewComponent : ImageComponent, IRenderable
    {
        public bool isOpen = false;

        /// <summary>
        /// Event fired whenever the view is opened. <br></br> Set it using <see cref="EventOpen"/> or <see cref="Events"/>.
        /// </summary>
        public UnityEvent onOpen { get; protected set; }
        
        /// <summary>
        /// Event fired whenever the view is closed. <br></br> Set it using <see cref="EventClose"/> or <see cref="Events"/>.
        /// </summary>
        public UnityEvent onClose { get; protected set; }

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        
        public bool isLocked = false;

        private InputAction _toggleAction;
        private InputAction _openAction;
        private InputAction _closeAction;

        private Button _button;

        public bool closesOnBackgroundTap = false;

        private ViewStackComponent _viewStackComponent;

        public override void Awake()
        {
            base.Awake();

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            if (Defaults.View.AutoAddEvents)
                Events();
            
            _button = gameObject.GetOrAddComponent<Button>();
            _button.transition = Selectable.Transition.None;
            _button.onClick.AddListener(OnBackgroundTap);
        }
    
        public override void Start()
        {
            base.Start();
            this.DisplayName("View");
            
            Color(Defaults.View.DefaultBackdropColor, Defaults.View.DefaultBackdropAlpha);
            
            if (isOpen) Open(); else Close();
        }

        /// <summary>
        /// Sets <see cref="isOpen"/> to true, resulting in the view to stay open on <see cref="Start"/>.
        /// </summary>
        /// <returns></returns>
        public ViewComponent StartOpen()
        {
            isOpen = true;
            return this;
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        public virtual void Render() { }
        
        /// <summary>
        /// Toggles between opening and closing the view based on <see cref="isOpen"/>.
        /// </summary>
        public virtual void ToggleOpenClose()
        {
            if (isOpen)
                Close();
            else
                Open();
        }
        private void ToggleOpenClose(InputAction.CallbackContext context) => ToggleOpenClose();

        /// <summary>
        /// Opens the view.
        /// </summary>
        public virtual void Open()
        {
            if (isLocked)
                return;
            
            _canvasGroup.alpha = 1f;
            RaycastTarget(true);

            isOpen = true;
            this.BringToFront();
            Render();

            onOpen?.Invoke();
           
            EnsureEnabledOfAllChildren(true);
        }
        
        /// <summary>
        /// Closes the view.
        /// </summary>
        public virtual void Close()
        {
            if (isLocked)
                return;
            
            _canvasGroup.alpha = 0f;
            RaycastTarget(false);
            
            isOpen = false;

            onClose?.Invoke();
            
            EnsureEnabledOfAllChildren(false);
        }

        private void EnsureEnabledOfAllChildren(bool e)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<IEnabled>()?.Enabled(e);
            }
        }

        /// <summary>
        /// Parents the components to the view.
        /// </summary>
        /// <param name="components">The components to parent.</param>
        /// <returns></returns>
        public ViewComponent Add(params BaseComponent[] components)
        {
            foreach (var baseComponent in components)
            {
                Add(baseComponent);
            }
            return this;
        }
        /// <summary>
        /// Parents the component to the view.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <returns></returns>
        public ViewComponent Add(BaseComponent component)
        {
            component.Parent(this);
            return this;
        }
        
        /// <summary>
        /// Links an input action to <see cref="ToggleOpenClose()"/>.
        /// </summary>
        /// <param name="toggle">The <see cref="InputAction"/> to link.</param>
        /// <returns></returns>
        public ViewComponent ToggleUsing(InputAction toggle)
        {
            _toggleAction = toggle;
            _toggleAction.performed += ToggleOpenClose;
            return this;
        }
        
        /// <summary>
        /// Links an input action to <see cref="Open"/>.
        /// </summary>
        /// <param name="open">The <see cref="InputAction"/> to link.</param>
        /// <returns></returns>
        public ViewComponent OpenUsing(InputAction open)
        {
            _openAction = open;
            _openAction.performed += _ => Open();
            return this;
        }
        
        /// <summary>
        /// Links an input action to <see cref="Close"/>.
        /// </summary>
        /// <param name="close">The <see cref="InputAction"/> to link.</param>
        /// <returns></returns>
        public ViewComponent CloseUsing(InputAction close)
        {
            _openAction = close;
            _openAction.performed += _ => Close();
            return this;
        }

        /// <summary>
        /// Sets <see cref="isLocked"/> to true. This disables opening and closing.
        /// </summary>
        public void Lock()
        {
            isLocked = true;
        }
        /// <summary>
        /// Sets <see cref="isLocked"/> to false. This re-enabled opening and closing if the view was previsouly locked.
        /// </summary>
        public void Unlock()
        {
            isLocked = false;
        }

        /// <summary>
        /// Toggles between <see cref="Lock"/> and <see cref="Unlock"/>.
        /// </summary>
        public void ToggleLock()
        {
            isLocked = !isLocked;
        }
        
        public void OnEnable()
        {
            _toggleAction?.Enable();
            _openAction?.Enable();
            _closeAction?.Enable();
        }
        public void OnDisable()
        {
            _toggleAction?.Disable();
            _openAction?.Disable();
            _closeAction?.Disable();
        }

        /// <summary>
        /// Automatically goes into <see cref="UI.FullScreen{T}"/> and parents the view to the canvas.
        /// </summary>
        /// <param name="canvas">The canvas to link.</param>
        /// <returns></returns>
        public ViewComponent Link(Canvas canvas)
        {
            _canvas = canvas;
            this.FullScreen(_canvas);
            return this.Parent(canvas);
        }

        /// <summary>
        /// Creates an event (<see cref="onOpen"/>) for when the view is opened.
        /// </summary>
        /// <returns></returns>
        public ViewComponent EventClose()
        {
            onOpen ??= new UnityEvent();
            return this;
        }
        
        /// <summary>
        /// Creates an event (<see cref="onClose"/>) for when the view is closed.
        /// </summary>
        /// <returns></returns>
        public ViewComponent EventOpen()
        {
            onClose ??= new UnityEvent();
            return this;
        }
        
        /// <summary>
        /// Creates events for opening and closing the view: <see cref="onOpen"/> and <see cref="onClose"/>.
        /// </summary>
        /// <returns></returns>
        public ViewComponent Events()
        {
            EventClose();
            return EventOpen();
        }

        private void OnBackgroundTap()
        {
            if (closesOnBackgroundTap && isOpen && !isLocked)
            {
                if (_viewStackComponent)
                {
                    if (_viewStackComponent.Peek() == this)
                    {
                        _viewStackComponent.Pop();
                        return;
                    }
                    
                    UCGUILogger.LogWarning($"<b>Closed View which is not on top of the view stack: {this}!</b>\nThis can lead to unwanted behaviours. Please make sure you open and close Views in a ViewStack in the intended order.");
                }
                Close();
            }
        }

        /// <summary>
        /// Invoked when a <see cref="ViewStackComponent"/> pushes this view.
        /// </summary>
        /// <param name="stackComponent">The stack pushing the view.</param>
        /// <returns></returns>
        public ViewComponent OnStackJoined(ViewStackComponent stackComponent)
        {
            _viewStackComponent = stackComponent;
            return this;
        }
        /// <summary>
        /// Invoked when a <see cref="ViewStackComponent"/> pops this view.
        /// </summary>
        /// <returns></returns>
        public ViewComponent OnStackLeft()
        {
            _viewStackComponent = null;
            return this;
        }

        public partial class ViewBuilder
        {
            private readonly ViewComponent _viewComponent;

            public ViewBuilder(ViewComponent viewComponent)
            {
                _viewComponent = viewComponent; 
            }
            public ViewBuilder(ViewComponent viewComponent, Canvas canvas) : this(viewComponent)
            {
                viewComponent.Link(canvas);
            }
            /// <inheritdoc cref="ViewComponent.ToggleUsing"/>
            public void ToggleUsing(InputAction toggle) => _viewComponent.ToggleUsing(toggle);
            
            /// <inheritdoc cref="ViewComponent.OpenUsing"/>
            public void OpenUsing(InputAction open) => _viewComponent.OpenUsing(open);
            /// <inheritdoc cref="ViewComponent.CloseUsing"/>
            public void CloseUsing(InputAction close) => _viewComponent.CloseUsing(close);

            /// <see cref="ViewComponent.Add(UCGUI.BaseComponent[])"/>
            public void Add(params BaseComponent[] contents) => _viewComponent.Add(contents);

            /// <inheritdoc cref="ViewComponent.Lock"/>
            public void Lock() => _viewComponent.Lock();

            /// <summary>
            /// Sets whether the view closes automatically when tapping on its background.
            /// </summary>
            /// <param name="closes">Boolean controlling <see cref="ViewComponent.closesOnBackgroundTap"/></param>.
            public void CloseOnBackgroundTap(bool closes = true) => _viewComponent.closesOnBackgroundTap = closes;
            
            /// <inheritdoc cref="ViewComponent.StartOpen"/>
            public void StartOpen() => _viewComponent.StartOpen();

            /// <inheritdoc cref="ViewComponent.EventOpen"/>
            public void EventOpen() => _viewComponent.EventOpen();
            
            /// <inheritdoc cref="ViewComponent.EventClose"/>
            public void EventClose() => _viewComponent.EventClose();
            
            /// <inheritdoc cref="ViewComponent.Events"/>
            public void Events() => _viewComponent.Events();
        }
    }
}