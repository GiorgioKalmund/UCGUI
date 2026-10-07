using JetBrains.Annotations;
using UCGUI.Support;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UCGUI
{
    public enum ViewStatus
    {
        Opened, Closed, Revealed, Hidden
    }
    
    /// UCGUI's framework for creation view-like components.
    /// Implements <see cref="IRenderable"/>.
    public abstract class AbstractViewComponent : ImageComponent, IRenderable 
    {
        public bool IsOpen { get; protected set; }

        #region Events

        #region OnOpen

        private UnityEvent _onOpen;

        /// <summary>
        /// Event fired whenever the view is opened.
        /// </summary>
        public UnityEvent OnOpen {
            get
            {
                _onOpen ??= new UnityEvent();
                return _onOpen;
            }
            protected set => _onOpen = value;
        }

        #endregion

        #region OnClose

        private UnityEvent _onClose;

        /// <summary>
        /// Event fired whenever the view is closed.
        /// </summary>
        public UnityEvent OnClose
        {
            get
            {
                _onClose ??= new UnityEvent();
                return _onClose;
            }
            protected set => _onClose = value;
        }

        #endregion
        
        #region OnStackReveal

        internal UnityEvent onStackReveal;

        /// <summary>
        /// Event fired whenever the view is part of a <see cref="UCGUI.ViewStackComponent"/> and has just been revealed.
        /// </summary>
        public UnityEvent OnStackReveal 
        {
            get
            {
                onStackReveal ??= new UnityEvent();
                return onStackReveal;
            }
            protected set => onStackReveal = value;
        }

        #endregion
        
        #region OnStackHide

        internal UnityEvent onStackHide;

        /// <summary>
        /// Event fired whenever the view is part of a <see cref="UCGUI.ViewStackComponent"/> and has just been hidden.
        /// </summary>
        public UnityEvent OnStackHide 
        {
            get
            {
                onStackHide ??= new UnityEvent();
                return onStackHide;
            }
            protected set => onStackHide = value;
        }

        #endregion
        
        #region OnStatusChanged

        internal UnityEvent<ViewStatus> onStatusChanged;

        /// <summary>
        /// Event fired when certain actions are performed according to the available <see cref="ViewStatus"/> options.
        /// </summary>
        public UnityEvent<ViewStatus> OnStatusChanged
        {
            get
            {
                onStatusChanged ??= new UnityEvent<ViewStatus>();
                return onStatusChanged;
            }
            set => onStatusChanged = value;
        }

        #endregion
        
        #endregion

        [CanBeNull] protected Canvas canvas;
        [HideInInspector] public CanvasGroup canvasGroup;

        protected Button backgroundClosingButton;
        
        /// <summary>
        /// Whether the view is locked. Locked views cannot be opened
        /// or closed.
        /// </summary>
        public bool IsLocked { get; protected set; }

        private InputAction _toggleAction;
        private InputAction _openAction;
        private InputAction _closeAction;

        /// <summary>
        /// If set to true will add a listener to the back pane of the view
        /// which is bound to <see cref="Close"/>.
        /// </summary>
        public bool ViewClosesOnBackgroundTap => backgroundClosingButton?.enabled ?? false;

        /// <summary>
        /// Reference to the <see cref="ViewStackComponent"/>, if the view has joined one.
        /// </summary>
        [CanBeNull] protected ViewStackComponent viewStackComponent;

        protected override void Awake()
        {
            base.Awake();

            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            IsOpen = Defaults.View.StartsOpen;
            
            Color(Defaults.View.DefaultBackdropColor);
            
            BeginParentContext(this);
            Create();
            EndParentContext();
        }

        /// <summary>
        /// Called during the 'Awake' phase of the Unity lifecycle. Build and configure your content here.
        /// </summary>
        protected abstract void Create();
        
        protected virtual void Start()
        {
            BeginParentContext(this);
            Initialize();
            EndParentContext();
            
            if (IsOpen) ForceOpen(); else ForceClose();
        }
        
        /// <summary>
        /// Called during the 'Start' phase of the Unity lifecycle and after UCGUI has initialized some defaults for the view.
        /// Initialize your contents here.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Sets <see cref="IsOpen"/> to true, resulting in the view to stay open on <see cref="Start"/>.
        /// </summary>
        /// <returns></returns>
        public AbstractViewComponent StartOpen()
        {
            IsOpen = true;
            return this;
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        public abstract void Render();
        
        /// <summary>
        /// Toggles between opening and closing the view based on <see cref="IsOpen"/>.
        /// </summary>
        public virtual void ToggleOpenClose()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }
        private void ToggleOpenClose(InputAction.CallbackContext context) => ToggleOpenClose();
        
        /// <summary>
        /// Internal shorthand handler which is invoked when this view is part of a ViewStack and becomes its new top element.
        /// Either when the view is pushed or when the previous top of the stack was popped and this view is next.
        /// </summary>
        public virtual void HandleViewStackReveal() { }
        
        /// <summary>
        /// Internal shorthand handler which is invoked when this view is part of a ViewStack stops being its new top element.
        /// Either when the view is popped or when another view is pushed on top of it.
        /// </summary>
        public virtual void HandleViewStackHide() { }

        /// <summary>
        /// Opens and re-renders the view.
        /// </summary>
        /// <remarks>Calls <see cref="ForceOpen"/>.</remarks>
        public virtual void Open()
        {
            if (IsLocked)
                return;
            
            ForceOpen();
        }

        /// <summary>
        /// Performs the actual opening of the view.
        /// <br></br>
        /// Invokes <see cref="OnOpen"/>.
        /// </summary>
        public void ForceOpen()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            RaycastTarget(true);

            IsOpen = true;
            this.BringToFront();

            _onOpen?.Invoke();
            onStatusChanged?.Invoke(ViewStatus.Opened);
           
            Render();
        }
        
        /// <summary>
        /// Closes the view if it is part of a <see cref="UCGUI.ViewStackComponent"/> and on top. If it is not part, will simply close the View.
        /// </summary>
        /// <remarks>Errors when the view is part of a <see cref="UCGUI.ViewStackComponent"/> and but isn't on top when trying to close it.</remarks>
        public virtual void Close()
        {
            if (IsOpen && !IsLocked)
            {
                if (viewStackComponent)
                {
                    if (IsOnTopOfViewStack())
                    {
                        viewStackComponent.Pop();
                        return;
                    }
                    
                    UCGUILogger.LogError($"<b>Attempting to close view which is not on top of the view stack: {this}!</b>" +
                                         $"\nThis is not allowed. Please make sure you open and close Views in a ViewStack in the intended order using \"ViewStack.Pop()\" or by calling \"CloseInViewStack()\"");
                    return;
                }
                ForceClose();
            }
        }
        
        /// <summary>
        /// Performs the actual closing of the view.
        /// <br></br>
        /// Invokes <see cref="OnClose"/>.
        /// </summary>
        public void ForceClose()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            RaycastTarget(false);
            
            IsOpen = false;

            _onClose?.Invoke();
            onStatusChanged?.Invoke(ViewStatus.Closed);
        }

        /// <summary>
        /// Parents the components to the view.
        /// </summary>
        /// <param name="components">The components to parent.</param>
        /// <returns></returns>
        public AbstractViewComponent Add(params BaseComponent[] components)
        {
            foreach (var baseComponent in components)
                baseComponent.Parent(this);
            return this;
        }
        
        /// <summary>
        /// Links an input action to <see cref="ToggleOpenClose()"/>.
        /// </summary>
        /// <param name="toggle">The <see cref="InputAction"/> to link.</param>
        /// <returns></returns>
        public virtual AbstractViewComponent ToggleUsing(InputAction toggle)
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
        public AbstractViewComponent OpenUsing(InputAction open)
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
        public AbstractViewComponent CloseUsing(InputAction close)
        {
            _openAction = close;
            _openAction.performed += _ => Close();
            return this;
        }

        /// <summary>
        /// Sets <see cref="IsLocked"/> to true. This disables opening and closing.
        /// </summary>
        public void Lock()
        {
            IsLocked = true;
        }
        /// <summary>
        /// Sets <see cref="IsLocked"/> to false. This re-enabled opening and closing if the view was previously locked.
        /// </summary>
        public void Unlock()
        {
            IsLocked = false;
        }

        /// <summary>
        /// Toggles between <see cref="Lock"/> and <see cref="Unlock"/>.
        /// </summary>
        public void ToggleLock()
        {
            IsLocked = !IsLocked;
        }
        
        public virtual void OnEnable()
        {
            _toggleAction?.Enable();
            _openAction?.Enable();
            _closeAction?.Enable();
        }
        public virtual void OnDisable()
        {
            _toggleAction?.Disable();
            _openAction?.Disable();
            _closeAction?.Disable();
        }

        /// <summary>
        /// Automatically applies <see cref="UI.Maximize{T}"/> and parents the view to the canvas.
        /// </summary>
        /// <param name="c">The canvas to link.</param>
        /// <returns></returns>
        public AbstractViewComponent Link(Canvas c)
        {
            canvas = c;
            this.Parent(c);
            return this;
        }


        /// <summary>
        /// Allows going back to this view instance inside the connected <see cref="UCGUI.ViewStackComponent"/>.
        /// <br></br> Preferably you should call <see cref="ViewStackComponent.PopUntil(AbstractViewComponent)"/>.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public bool BackToSelfInViewStack()
        {
            return viewStackComponent?.PopUntil(this) ?? false;
        }

        /// <summary>
        /// Invoked when a <see cref="UCGUI.ViewStackComponent"/> pushes this view.
        /// </summary>
        /// <param name="stackComponent">The stack pushing the view.</param>
        /// <remarks>WARNING: It is recommended to not call this function by itself, rather
        /// call <see cref="ViewStackComponent.Push"/> with this element. You might encounter an inconsistent state otherwise!
        /// </remarks>
        public AbstractViewComponent JoinStack(ViewStackComponent stackComponent)
        {
            viewStackComponent = stackComponent;
            onStackReveal?.Invoke();
            onStatusChanged?.Invoke(ViewStatus.Revealed);
            HandleViewStackReveal();
            return this;
        }
        /// <summary>
        /// Invoked when a <see cref="UCGUI.ViewStackComponent"/> pops this view.
        /// </summary>
        /// <remarks>WARNING: It is recommended to not call this function by itself, rather
        /// call <see cref="ViewStackComponent.Pop"/> when this element is on top. You might encounter an inconsistent state otherwise!
        /// </remarks>
        public AbstractViewComponent LeaveStack()
        {
            onStackHide?.Invoke();
            onStatusChanged?.Invoke(ViewStatus.Hidden);
            HandleViewStackHide();
            viewStackComponent = null;
            return this;
        }

        /// <summary>
        /// Whether the view is part of a view stack and on top of it.
        /// </summary>
        /// <returns></returns>
        public bool IsOnTopOfViewStack()
        {
            return viewStackComponent != null && viewStackComponent.Peek() == this;
        }
        
        #if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            if (debugOptions.HasFlag(DebugOptions.TextOnly))
            {
                RectTransform rect = gameObject.GetComponent<RectTransform>();
                Handles.Label(transform.position + new Vector3(-rect.sizeDelta.x / 2, rect.sizeDelta.y / 2, 0),  $"\nOpen:{IsOpen}\nLocked:{IsLocked}\nClosesOnTap:{ViewClosesOnBackgroundTap}", Defaults.Debug.DebugRed(8));
            }
        }
        #endif


        public AbstractViewComponent ClosesOnBackgroundTap(bool closes, bool softDestroy = false)
        {
            if (closes == ViewClosesOnBackgroundTap)
            {
                UCGUILogger.LogWarning($"View '{DisplayName}' is already in background closing state '{ViewClosesOnBackgroundTap}'. Duplicate calls to the same state are ignored.");
                return this;
            }
        

            if (closes)
            {
                if (!backgroundClosingButton)
                {
                    var parent = UI.Blank().Maximize().IgnoreLayout()
                        .Parent(this).BringToBack()
                        .DisplayName("Background Closing Area");
                    parent.gameObject.AddComponent<Image>().color = UnityEngine.Color.clear;
                    backgroundClosingButton = parent.gameObject.AddComponent<Button>();
                    backgroundClosingButton.transition = Selectable.Transition.None;
                    backgroundClosingButton.onClick.AddListener(Close);
                }

                backgroundClosingButton.enabled = true;
            }
            else if (backgroundClosingButton)
            {
                if (softDestroy)
                    backgroundClosingButton.enabled = false;
                else
                    Destroy(backgroundClosingButton.gameObject);
            }
            

            return this;
        }

        /// <summary>
        /// Minimal builder for small and simple views.
        /// </summary>
        public class ViewBuilder
        {
            private readonly AbstractViewComponent _view;

            public ViewBuilder(AbstractViewComponent view, Canvas canvas = null)
            {
                _view = view;
                if (canvas)
                    view.Link(canvas);
            }
            
            /// <inheritdoc cref="AbstractViewComponent.ToggleUsing"/>
            public void ToggleUsing(InputAction toggle) => _view.ToggleUsing(toggle);
            
            /// <inheritdoc cref="AbstractViewComponent.OpenUsing"/>
            public void OpenUsing(InputAction open) => _view.OpenUsing(open);
            /// <inheritdoc cref="AbstractViewComponent.CloseUsing"/>
            public void CloseUsing(InputAction close) => _view.CloseUsing(close);

            /// <see cref="AbstractViewComponent.Add(UCGUI.BaseComponent[])"/>
            public void Add(params BaseComponent[] contents) => _view.Add(contents);

            /// <inheritdoc cref="AbstractViewComponent.Lock"/>
            public void Lock() => _view.Lock();

            /// <summary>
            /// Sets whether the view closes automatically when tapping on its background.
            /// </summary>
            /// <param name="closes">Boolean controlling <see cref="AbstractViewComponent.ViewClosesOnBackgroundTap"/>.</param>
            public void CloseOnBackgroundTap(bool closes = true) => _view.ClosesOnBackgroundTap(closes);
        }
    }
}