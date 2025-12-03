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
    /// <item><description><see cref="Lock"/> - Locks opening and closing behaviour.</description></item>.
    /// <item><description><see cref="Unlock"/> - Unlocks opening and closing behaviour.</description></item>.
    /// <item><description><see cref="ToggleLock"/> - Toggles between <see cref="Lock"/> and <see cref="Unlock"/>.</description></item>.
    /// <item><description><see cref="IsOnTopOfViewStack"/> - Returns whether this view is on top of its connected <see cref="ViewStackComponent"/>.</description></item>.
    /// </list>
    /// Events:
    /// <list type="bullet">
    /// <item><description><see cref="OnOpen"/> - Invoked when the view is opened and either <see cref="EventOpen()"/> or <see cref="Events"/> has been called.</description></item>.
    /// <item><description><see cref="OnClose"/> - Invoked when the view is closed and either <see cref="EventOpen()"/> or <see cref="Events"/> has been called.</description></item>.
    /// </list>
    /// <para>
    /// Implements <see cref="IRenderable"/>.
    /// </para>
    public abstract class AbstractViewComponent : ImageComponent, IRenderable
    {
        protected AbstractViewComponent() {}
        public bool IsOpen { get; protected set; } = Defaults.View.StartsOpen;

        /// <summary>
        /// Event fired whenever the view is opened. <br></br> Set it using <see cref="EventOpen()"/> or <see cref="Events"/>.
        /// </summary>
        public UnityEvent OnOpen { get; protected set; }
        
        /// <summary>
        /// Event fired whenever the view is closed. <br></br> Set it using <see cref="EventOpen()"/> or <see cref="Events"/>.
        /// </summary>
        public UnityEvent OnClose { get; protected set; }

        [CanBeNull] protected Canvas canvas;
        public CanvasGroup canvasGroup;
        
        public bool IsLocked { get; protected set; }

        private InputAction _toggleAction;
        private InputAction _openAction;
        private InputAction _closeAction;

        private Button _button;

        public bool ClosesOnBackgroundTap { get; protected set; }

        private ViewStackComponent _viewStackComponent;

        public override void Awake()
        {
            base.Awake();

            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            if (Defaults.View.AutoAddEvents)
                Events();

            _button = gameObject.GetOrAddComponent<Button>();
            _button.transition = Selectable.Transition.None;
            _button.onClick.AddListener(() =>
            {
                if (ClosesOnBackgroundTap)
                    Close();
            });
            
            Color(Defaults.View.DefaultBackdropColor);
          
            CreateView();
        }

        public abstract void CreateView();
        
        public override void Start()
        {
            base.Start();
            this.DisplayName("View");
            
            if (IsOpen) ForceOpen(); else ForceClose();
        }

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
        /// Performs the actual opnening of the view.
        /// <br></br>
        /// Invokes <see cref="OnOpen"/>.
        /// </summary>
        public void ForceOpen()
        {
            if (IsLocked)
                return;
            
            canvasGroup.alpha = 1f;
            RaycastTarget(true);

            IsOpen = true;
            this.BringToFront();
            Render();

            OnOpen?.Invoke();
           
            EnsureEnabledOfAllChildren(true);
        }
        
        /// <summary>
        /// Closes the view if it is part of a <see cref="ViewStackComponent"/> and on top. If it is not part, will simply close the View.
        /// </summary>
        /// <remarks>Errors when the view is part of a <see cref="ViewStackComponent"/> and but isn't on top when trying to close it.</remarks>
        public virtual void Close()
        {
            if (IsOpen && !IsLocked)
            {
                if (_viewStackComponent)
                {
                    if (IsOnTopOfViewStack())
                    {
                        _viewStackComponent.Pop();
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
            if (IsLocked)
                return;
            
            canvasGroup.alpha = 0f;
            RaycastTarget(false);
            
            IsOpen = false;

            OnClose?.Invoke();
            
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
        /// Sets <see cref="IsLocked"/> to false. This re-enabled opening and closing if the view was previsouly locked.
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
        /// Creates an event (<see cref="OnOpen"/>) for when the view is opened.
        /// </summary>
        /// <returns></returns>
        public AbstractViewComponent EventOpen()
        {
            OnOpen ??= new UnityEvent();
            return this;
        }
        
        /// <summary>
        /// Creates an event (<see cref="OnClose"/>) for when the view is closed.
        /// </summary>
        /// <returns></returns>
        public AbstractViewComponent EventClose()
        {
            OnClose ??= new UnityEvent();
            return this;
        }
        
        /// <summary>
        /// Creates events for opening and closing the view: <see cref="OnOpen"/> and <see cref="OnClose"/>.
        /// </summary>
        /// <returns></returns>
        public AbstractViewComponent Events()
        {
            EventClose();
            return EventOpen();
        }


        /// <summary>
        /// Allows going back to this view instance inside the connected <see cref="ViewStackComponent"/>.
        /// <br></br> Preferably you should call <see cref="ViewStackComponent.PopUntil(AbstractViewComponent)"/>.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public virtual bool BackToSelfInViewStack()
        {
            return _viewStackComponent.PopUntil(this);
        }

        /// <summary>
        /// Invoked when a <see cref="ViewStackComponent"/> pushes this view.
        /// </summary>
        /// <param name="stackComponent">The stack pushing the view.</param>
        /// <returns></returns>
        public AbstractViewComponent OnStackJoined(ViewStackComponent stackComponent)
        {
            _viewStackComponent = stackComponent;
            return this;
        }
        /// <summary>
        /// Invoked when a <see cref="ViewStackComponent"/> pops this view.
        /// </summary>
        /// <returns></returns>
        public AbstractViewComponent OnStackLeft()
        {
            _viewStackComponent = null;
            return this;
        }

        public bool IsOnTopOfViewStack()
        {
            return _viewStackComponent != null && _viewStackComponent.Peek() == this;
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

        public partial class ViewBuilder
        {
            private readonly AbstractViewComponent abstractViewComponent;

            public ViewBuilder(AbstractViewComponent abstractViewComponent, Canvas canvas = null)
            {
                this.abstractViewComponent = abstractViewComponent;
                if (canvas)
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
            
            /// <inheritdoc cref="AbstractViewComponent.EventOpen"/>
            public void EventOpen() => abstractViewComponent.EventOpen();
            
            /// <inheritdoc cref="AbstractViewComponent.EventClose"/>
            public void EventClose() => abstractViewComponent.EventClose();
            
            /// <inheritdoc cref="AbstractViewComponent.Events"/>
            public void Events() => abstractViewComponent.Events();
        }
    }
}