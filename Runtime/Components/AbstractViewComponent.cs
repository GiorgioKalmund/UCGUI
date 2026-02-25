using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UCGUI
{
    /// UCGUI's framework for creation view-like components.
    /// <br></br>
    /// <br></br>
    /// Will default to <see cref="Close"/> on <see cref="Start"/>. Applies <see cref="Defaults.View.DefaultBackdropColor"/> to the background by default.
    /// <br></br>
    /// <br></br>
    /// Variables:
    /// <list type="bullet">
    /// <item><description><see cref="IsOpen"/> - Whether the view is open.</description></item>.
    /// <item><description><see cref="IsLocked"/> - Whether the view is locked If <i>true</i>, the view can neither be opened nor closed.</description></item>.
    /// <item><description><see cref="ClosesOnBackgroundTap"/> - Determines whether the view should close when clicking on the background.</description></item>.
    /// </list>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="Open"/> - Opens the view.</description></item>.
    /// <item><description><see cref="Close"/> - Closes the view.</description></item>.
    /// <item><description><see cref="ToggleOpenClose"/> - Toggles between opening and closing the view.</description></item>.
    /// <item><description><see cref="StartOpen"/> - Will <b>not</b> close on <see cref="Start"/>.</description></item>.
    /// <item><description><see cref="OpenUsing"/> - Allows the user to supply and <see cref="InputAction"/> to toggle the opening of the view.</description></item>.
    /// <item><description><see cref="CloseUsing"/> - Allows the user to supply and <see cref="InputAction"/> to toggle the closing of the view.</description></item>.
    /// <item><description><see cref="ToggleUsing"/> - Allows the user to supply and <see cref="InputAction"/> to toggle the opening and closing of the view.</description></item>.
    /// <item><description><see cref="Lock"/> - Locks opening and closing behavior.</description></item>.
    /// <item><description><see cref="Unlock"/> - Unlocks opening and closing behavior.</description></item>.
    /// <item><description><see cref="ToggleLock"/> - Toggles between <see cref="Lock"/> and <see cref="Unlock"/>.</description></item>.
    /// <item><description><see cref="IsOnTopOfViewStack"/> - Returns whether this view is on top of its connected <see cref="UCGUI.ViewStackComponent"/>.</description></item>.
    /// </list>
    /// Events:
    /// <list type="bullet">
    /// <item><description><see cref="OnOpen"/> - Invoked when the view is opened.</description></item>.
    /// <item><description><see cref="OnClose"/> - Invoked when the view is closed.</description></item>.
    /// <item><description><see cref="OnStackReveal"/> - Invoked when the view is part of a <see cref="UCGUI.ViewStackComponent"/> and has just been revealed.</description></item>.
    /// <item><description><see cref="OnStackHide"/> - Invoked when the view is part of a <see cref="UCGUI.ViewStackComponent"/> and has just been hidden.</description></item>.
    /// </list>
    /// <para>
    /// Implements <see cref="IRenderable"/>.
    /// </para>
    public abstract class AbstractViewComponent : ImageComponent, IRenderable
    {
        protected AbstractViewComponent() {}
        public bool IsOpen { get; protected set; } = Defaults.View.StartsOpen;

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
        
        private UnityEvent _onStackReveal;

        /// <summary>
        /// Event fired whenever the view is part of a <see cref="UCGUI.ViewStackComponent"/> and has just been revealed.
        /// </summary>
        public UnityEvent OnStackReveal 
        {
            get
            {
                _onStackReveal ??= new UnityEvent();
                return _onStackReveal;
            }
            protected set => _onStackReveal = value;
        }

        #endregion
        
        #region OnStackHide
        
        private UnityEvent _onStackHide;

        /// <summary>
        /// Event fired whenever the view is part of a <see cref="UCGUI.ViewStackComponent"/> and has just been hidden.
        /// </summary>
        public UnityEvent OnStackHide 
        {
            get
            {
                _onStackHide ??= new UnityEvent();
                return _onStackHide;
            }
            protected set => _onStackHide = value;
        }

        #endregion
        
        #endregion

        [CanBeNull] protected Canvas canvas;
        public CanvasGroup canvasGroup;
        
        /// <summary>
        /// Whether the view is locked. Locked views cannot be opened
        /// or closed.
        /// </summary>
        public bool IsLocked { get; protected set; }

        private InputAction _toggleAction;
        private InputAction _openAction;
        private InputAction _closeAction;

        private Button _button;

        /// <summary>
        /// If set to true will add a listener to the back pane of the view
        /// which is bound to <see cref="Close"/>.
        /// </summary>
        public bool ClosesOnBackgroundTap { get; protected set; }

        /// <summary>
        /// Reference to the <see cref="ViewStackComponent"/>, if the view has joined one.
        /// </summary>
        [CanBeNull] protected ViewStackComponent viewStackComponent;

        public override void Awake()
        {
            base.Awake();
            DisplayName = "AbstractView";

            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            _button = gameObject.GetOrAddComponent<Button>();
            _button.transition = Selectable.Transition.None;
            
            Color(Defaults.View.DefaultBackdropColor);
            
            Create();
        }

        /// <summary>
        /// Called during the 'Awake' phase of the Unity lifecycle. Build and configure your content here.
        /// </summary>
        protected abstract void Create();
        
        public override void Start()
        {
            base.Start();
            
            if (ClosesOnBackgroundTap)
                _button.onClick.AddListener(Close);
            
            Initialize();
            
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
            RaycastTarget(true);

            IsOpen = true;
            this.BringToFront();

            OnOpen?.Invoke();
           
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
            RaycastTarget(false);
            
            IsOpen = false;

            OnClose?.Invoke();
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
        public AbstractViewComponent Add(params BaseComponent[] components)
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
        public virtual AbstractViewComponent Add(BaseComponent component)
        {
            component.Parent(this);
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
        public virtual bool BackToSelfInViewStack()
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
            OnStackReveal.Invoke();
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
            OnStackHide.Invoke();
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
                Handles.Label(transform.position + new Vector3(-rect.sizeDelta.x / 2, rect.sizeDelta.y / 2, 0),  $"\nOpen:{IsOpen}\nLocked:{IsLocked}\nClosesOnTap:{ClosesOnBackgroundTap}", Defaults.Debug.DebugRed(8));
            }
        }
        #endif

        /// <summary>
        /// Minimal builder for small and simple views.
        /// </summary>
        public class ViewBuilder
        {
            private readonly AbstractViewComponent abstractViewComponent;

            public ViewBuilder(AbstractViewComponent abstractViewComponent, Canvas canvas = null)
            {
                this.abstractViewComponent = abstractViewComponent;
                abstractViewComponent.Link(canvas);
            }
            
            /// <inheritdoc cref="AbstractViewComponent.ToggleUsing"/>
            public void ToggleUsing(InputAction toggle) => abstractViewComponent.ToggleUsing(toggle);
            
            /// <inheritdoc cref="AbstractViewComponent.OpenUsing"/>
            public void OpenUsing(InputAction open) => abstractViewComponent.OpenUsing(open);
            /// <inheritdoc cref="AbstractViewComponent.CloseUsing"/>
            public void CloseUsing(InputAction close) => abstractViewComponent.CloseUsing(close);

            /// <see cref="AbstractViewComponent.Add(UCGUI.BaseComponent[])"/>
            public void Add(params BaseComponent[] contents) => abstractViewComponent.Add(contents);

            /// <inheritdoc cref="AbstractViewComponent.Lock"/>
            public void Lock() => abstractViewComponent.Lock();

            /// <summary>
            /// Sets whether the view closes automatically when tapping on its background.
            /// </summary>
            /// <param name="closes">Boolean controlling <see cref="AbstractViewComponent.ClosesOnBackgroundTap"/>.</param>
            public void CloseOnBackgroundTap(bool closes = true) => abstractViewComponent.ClosesOnBackgroundTap = closes;
        }
    }
}