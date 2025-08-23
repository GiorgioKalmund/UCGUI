using System.Collections.Generic;
using TMPro;
using UCGUI.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UCGUI
{
    public class WindowComponent : BaseComponent, IPointerDownHandler, IFocusable, IBeginDragHandler, IDragHandler, IRenderable
    {
        // -- Canvas sizes stored to avoid duplicate calculations -- //
        protected static float CanvasWidth;
        protected static float CanvasHeight;
        protected float HeaderHeight = 20f;
        protected float MinimumWindowShowingHeight = 20f;

        private static readonly Vector2 DefaultMinimumWindowSize = new Vector2(200, 100);
        
        // -- Border offset to allow for margins around the screen -- //
        private float _borderOffset = 0f;

        // Key for showing / hiding the window, initial state -- //
        private InputAction _toggleInputAction;
        private bool _startOfHidden;
        public bool Hidden => !WindowBase?.gameObject.activeSelf ?? true;
        
        // Mask
        private RectMask2D _rectMask2D;
        
        // -- Subcomponents -- //
        protected ImageComponent WindowBase;
        private ButtonComponent _collapseButton;
        protected ImageComponent Header;
        private TextComponent _headerText;
        
        // -- Dragging -- // 
        protected bool AllowDragging = true;
        
        private bool _collapsed = false;
        
        // Focus
        public int FocusGroup { get; protected set; } = 0;

        public List<MonoBehaviour> contents = new List<MonoBehaviour>();

        public override void Awake()
        {
            base.Awake();

            // ===== WINDOW BASE-- Parent of all subcomponents to allow hiding == //
            WindowBase = ComponentExtension.N<ImageComponent>(transform, "Window Base").Pivot(PivotPosition.UpperLeft, true);
            // ==================================================================== //
            WindowBase.AddLayoutElement();
            WindowBase.MinimumSize(DefaultMinimumWindowSize);
            
            Header = ComponentExtension.N<ImageComponent>(WindowBase, "Header")
                    .Pivot(PivotPosition.UpperCenter)
                    .AnchorMin(0, 1)
                    .AnchorMax(1, 1)
                    .Color(Color.rebeccaPurple)
                ;
            Header.AddLayoutElement().ignoreLayout = true;
            Header.AddHorizontalLayout(10, TextAnchor.MiddleLeft);

            _collapseButton = ComponentExtension.N<ButtonComponent>(Header, "Collapse Toggle")
                .Create(action:ToggleCollapse)
                .Sprite(ImageService.GetSpriteFromAsset("gui_assets", "right_arrow"))
                .Cast<ButtonComponent>()
                ;
            
            _headerText = ComponentExtension.N<TextComponent>(Header)
                    .Pivot(PivotPosition.MiddleLeft)
                    .AnchoredTo(PivotPosition.MiddleRight)
                    .FontSize(HeaderHeight - 7)
                    .VAlignment(VerticalAlignmentOptions.Middle)
                    .OverflowMode(TextOverflowModes.Ellipsis)
                    .Color(Color.gray1)
                    .AddFitter(ScrollViewDirection.Horizontal)
                    .Cast<TextComponent>()
                ;
        }

        public virtual WindowComponent Build(string title, InputAction action = null)
        {
            _toggleInputAction = action;
            _toggleInputAction?.Enable();
            Title(title);
            return this;
        }

        public WindowComponent Title(string title)
        {
            _headerText.Text(title);
            return this;
        }

        private void OnEnable()
        {
            _toggleInputAction?.Enable();
        }

        public virtual void Start()
        {
            Render();
            Expand();
            this.SafeDisplayName("BaseWindowComponent");
            
            if (_startOfHidden)
                Minimize();
            
            if (_toggleInputAction != null)
                _toggleInputAction.performed += HandleOpenAndMinimize;
            
            // Make header stretch
            Header.Stretch();
        }

        private void OnDisable()
        {
            _toggleInputAction?.Disable();
        }

        // To make support other subcomponent showing & hiding appropriately, please make sure to never parent to 'transform', but always to 'WindowBase'!
        private void HandleOpenAndMinimize(InputAction.CallbackContext callback)
        {
            if (Hidden)
                Open();
            else 
                Minimize();
        }
        public virtual void Open()
        {
            WindowBase.SetActive(true);
            this.BringToFront();
        }
        public virtual void Minimize()
        {
            WindowBase.SetActive(false);
        }

        public void ToggleCollapse()
        {
            this.Focus();
            if (_collapsed)
                Expand();
            else 
                Collapse();
        }

        public virtual void Collapse()
        {
            _collapsed = true;
            foreach (var monoBehaviour in contents)
            {
                monoBehaviour.gameObject.SetActive(false);
            }

            _collapseButton.Rotation(-90);
        }
        public virtual void Expand()
        {
            _collapsed = false;
            foreach (var monoBehaviour in contents)
            {
                monoBehaviour.gameObject.SetActive(true);
            }

            _collapseButton.Rotation(0);
        }

        public WindowComponent RecalculateSizes()
        {
            CanvasWidth = GUIService.GetCanvasWidth() / 2;
            CanvasHeight= GUIService.GetCanvasHeight() / 2;
            return this;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            this.Focus();
            RecalculateSizes();
        }
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            this.Focus();
            RecalculateSizes();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!AllowDragging)
                return;
            
            var pos = GetRect().anchoredPosition;
            Vector2 wantToBePos = (pos + eventData.delta);
            Vector2 willBePos = pos;

            if (Mathf.Abs(wantToBePos.x) < CanvasWidth - this.GetWidth() / 2 - _borderOffset)
            {
                willBePos.x = wantToBePos.x;
            }
            if (Mathf.Abs(wantToBePos.y) < CanvasHeight - this.GetHeight() / 2 - MinimumWindowShowingHeight - _borderOffset)
            {
                willBePos.y = wantToBePos.y;
            }

            GetRect().anchoredPosition = willBePos;
        }

        public virtual void HandleFocus()
        {
            this.BringToFront();
            Header.Alpha(1f);
        }

        public virtual void HandleUnfocus()
        {
            Header.Alpha(0.4f);
        }

        public WindowComponent Padding(float padding)
        {
            return Padding(PaddingSide.All, padding);
        }
        public WindowComponent Padding(PaddingSide side, float padding)
        {
            WindowBase.Padding(side, padding);
            return this;
        }
        
        public virtual void Render()
        {
            RenderHeader();
            RenderContent();
        }
        
        public virtual void RenderHeader()
        {
            Header.Size(this.GetWidth(), HeaderHeight);
            _headerText.Size(this.GetWidth() - HeaderHeight, HeaderHeight)
                .Padding(PaddingSide.Horizontal, 10);
            _collapseButton.Size(HeaderHeight, HeaderHeight);
        }
        public virtual void RenderContent()
        {
            HandleSizeChanged(this.GetWidth(), this.GetHeight());
        }

        public WindowComponent StartHidden(bool hidden = true)
        {
            _startOfHidden = hidden;
            return this;
        }

        public WindowComponent NoHeader()
        {
            HeaderHeight = 0;
            Header.SetActive(false);
            return this;
        }

        public WindowComponent SetBase(Sprite sprite)
        {
            WindowBase.Sprite(sprite);
            return this;
        }
        
        public WindowComponent SetBase(Color color)
        {
            WindowBase.Sprite((Sprite)null);
            WindowBase.Color(color);
            return this;
        }
        
        public WindowComponent SetHeaderColor(Color color)
        {
            Header.Color(color);
            return this;
        }

        public virtual int GetFocusGroup()
        {
            return FocusGroup;
        }

        public virtual void SetFocusGroup(int group)
        {
            FocusGroup = group;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            style.fontSize = 14; 
            Handles.Label(transform.position, GetFocusGroup() + "", style);
        }
        #endif

        protected virtual WindowComponent AddContent(BaseComponent component)
        {
            component.Parent(WindowBase);
            contents.Add(component);
            return this;
        }

        protected void RemoveContent(BaseComponent component)
        {
            var c = contents.Find(c1 => c1.Equals(component));
            if (c)
            {
                contents.Remove(c);
                if (c is BaseComponent baseC)
                {
                    baseC.Parent((Transform)null);
                }
            }
        }
        
        
        public class WindowBuilder
        {
            private WindowComponent _window;
            public WindowBuilder(WindowComponent window) { _window = window; }

            public void VerticalLayout(float spacing = 0, RectOffset padding = null)
            {
                _window.WindowBase.AddVerticalLayout(spacing, TextAnchor.UpperLeft);
                if (padding == null)
                    padding = new RectOffset(0, 0, (int)_window.HeaderHeight, 0);
                else
                    padding.top += (int)_window.HeaderHeight;
                
                _window.WindowBase.Padding(padding, ScrollViewDirection.Vertical);
            }
            
            public void Header(string title)
            {
                _window.Title(title);
            }
            public void Header(string title, Color color, float alpha = 1f)
            {
                Header(title);
                _window.Header.Color(color, alpha);
            }
            public void Header(string title, Sprite sprite, Image.Type type, float ppum)
            {
                Header(title);
                _window.Header.Sprite(sprite, type, ppum);
            }

            public void Background(Sprite sprite, Image.Type type, float ppum = 1f)
            {
                _window.WindowBase.Sprite(sprite, type, ppum);
            }
            public void Background(Color color, float alpha)
            {
                _window.WindowBase.Color(color, alpha);
            }
            
            public virtual void Text(string text, Color? color = null)
            {
                Add(UCGUI.Text(text, color ?? Color.gray1).FontSize(18).FitToContents().NoWrap());
            }

            public virtual void Add(params BaseComponent[] component)
            {
                foreach (var baseWindowComponent in component)
                {
                    _window.AddContent(baseWindowComponent);
                }
            }

            public void FitToContents(ScrollViewDirection direction)
            {
                _window.WindowBase.AddFitter(direction);
            }

            public void NoFitting()
            {
                _window.WindowBase.ContentSizeFitter.enabled = false;
            }

            public void MinimumDimensions(Vector2 minimumDim)
            {
                _window.MinimumSize(minimumDim);
            }
        }
    }
}