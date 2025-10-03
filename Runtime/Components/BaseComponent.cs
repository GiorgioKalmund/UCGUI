using UCGUI.Services;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public partial class BaseComponent : MonoBehaviour, ICopyable<BaseComponent>
    {
        
#if DEBUG
        [Header("Debug")]
        [SerializeField] protected DebugOptions debugOptions = DebugOptions.None;
        protected virtual void OnDrawGizmos()
        {
            if (debugOptions.HasFlag(DebugOptions.RectOnly))
            {
                RectTransform rect = gameObject.GetComponent<RectTransform>();
                Gizmos.color = Color.red;
                DrawRect(rect);
            }
            if (debugOptions.HasFlag(DebugOptions.TextOnly))
            {
                RectTransform rect = gameObject.GetComponent<RectTransform>();
                var style = new GUIStyle();
                style.normal.textColor = Color.red;
                style.fontSize = 8;
                Handles.Label(transform.position + new Vector3(-rect.sizeDelta.x / 2, rect.sizeDelta.y / 2, 0),  $"{rect.sizeDelta.x}x{rect.sizeDelta.y}", style);
            }
        }
        protected virtual void OnDrawGizmosSelected()
        {
            if (debugOptions.HasFlag(DebugOptions.RectOnly))
            {
                Gizmos.color = Color.green;
                DrawRect(gameObject.GetComponent<RectTransform>());
            }
        }

        public BaseComponent DebugMode(DebugOptions options)
        {
            debugOptions = options;
            return this;
        }
        
        protected void DrawRect(RectTransform rect)
        {
            Gizmos.DrawWireCube(
                rect.position,
                new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, 0.01f)
            );
        }
#endif
        
        private RectTransform _rect;
        
        [HideInInspector] public bool paddingApplied = false;
        [HideInInspector] public bool posApplied = false;

        private string _displayName = "BaseComponent";
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                gameObject.name = value;
            }
        }
    
        protected float CanvasWidthFactor => GUIService.WidthScale;
        protected float CanvasHeightFactor => GUIService.HeightScale;


        [HideInInspector] public LayoutElement layoutElement;
        public HorizontalLayoutGroup HorizontalLayout { get; protected set; }
        public VerticalLayoutGroup VerticalLayout { get; protected set; }
        public ContentSizeFitter ContentSizeFitter { get; protected set; }
        
        
        public virtual void Awake()
        {
            _rect = gameObject.GetOrAddComponent<RectTransform>();
        }
    
        public RectTransform GetRect()
        {
            if (!_rect)
            {
                UCGUILogger.LogWarning(gameObject.name  +": RectTransform is null! Did you forget to call 'base.Awake()' somewhere or does the GameObject not have a RectTransform attached?");
                _rect = gameObject.GetComponent<RectTransform>();
            }
            return _rect;
        }

        public virtual BaseComponent HandleSizeChanged(float x, float y)
        {
            return this;
        }
        

        private void OnValidate()
        {
            this.SafeDisplayName(_displayName);
        }
        
        
        public LayoutElement AddLayoutElement()
        {
            if (layoutElement)
                return layoutElement;
            layoutElement = gameObject.GetOrAddComponent<LayoutElement>();
            return layoutElement;
        }

        public LayoutElement AddLayoutElement(float minWidth, float minHeight)
        {
            AddLayoutElement();
            MinimumSize(minWidth, minHeight);
            return layoutElement;
        }

        public BaseComponent IgnoreLayout(bool ignore = true)
        {
            if (!layoutElement)
                AddLayoutElement();
            layoutElement.ignoreLayout = ignore;
            return this;
        }
        
        public BaseComponent MinimumSize(Vector2 size)
        {
            if (!layoutElement)
                AddLayoutElement();
            layoutElement.minWidth = size.x;
            layoutElement.minHeight = size.y;
            return this;
        }
        public BaseComponent MinimumSize(float x, float y)
        {
            return MinimumSize(new Vector2(x, y));
        }

        public Vector2 GetMinimumSize()
        {
            if (!layoutElement)
                AddLayoutElement();
            return new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
        }
        
        
        public static void CopyLayouts(BaseComponent other, BaseComponent copyComponent)
        {
            if (other.HorizontalLayout)
            {
                copyComponent.AddHorizontalLayout();
                copyComponent.HorizontalLayout.CopyFrom(other.HorizontalLayout);
            }
            if (other.VerticalLayout)
            {
                copyComponent.AddVerticalLayout();
                copyComponent.VerticalLayout.CopyFrom(other.VerticalLayout);
            }
        }
        
        public void CopyRect(RectTransform rect, BaseComponent component, bool copyPos = true)
        {
            Vector2 originalPos = component.GetPos();
            component.LocalScale(rect.localScale);
            component.GetRect().localRotation = rect.localRotation;
            if (copyPos)
            {
                component.Pivot(rect.pivot);
                component.AnchorMin(rect.anchorMin);
                component.AnchorMax(rect.anchorMax);
                component.OffsetMin(rect.offsetMin);
                component.OffsetMax(rect.offsetMax);
                component.Pos(rect.anchoredPosition);
            }
            else
            {
                component.Pos(originalPos);
            }
            component.Size(rect.sizeDelta);
        }
        
        public void CopyLayoutElement(BaseComponent otherComponent, BaseComponent component)
        {
            LayoutElement other = otherComponent.GetComponent<LayoutElement>();
            if (other)
            {
                var newLayoutElement = component.gameObject.GetOrAddComponent<LayoutElement>();
                newLayoutElement.preferredWidth = other.preferredWidth;
                newLayoutElement.preferredHeight = other.preferredHeight;
                newLayoutElement.minWidth = other.minWidth;
                newLayoutElement.minHeight= other.minHeight;
                newLayoutElement.layoutPriority = other.layoutPriority;
                newLayoutElement.ignoreLayout = other.ignoreLayout;
                newLayoutElement.enabled = other.enabled;
            }
        }
        
        protected T CreateLayout<T>(GameObject obj, float spacing, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false) where T : HorizontalOrVerticalLayoutGroup
        {
            var layout = obj.GetOrAddComponent<T>();
            layout.spacing = spacing;
            layout.childAlignment = childAlignment;
            
            layout.childControlWidth = childControlWidth;
            layout.childControlHeight = childControlHeight;
            layout.childForceExpandWidth = childForceExpandWidth;
            layout.childForceExpandHeight = childForceExpandHeight;

            layout.reverseArrangement = reverseArrangement;
            return layout;
        }

        public BaseComponent AddLayout(ScrollViewDirection direction, float spacing = 0, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false)
        {
            if (direction.HasFlag(ScrollViewDirection.Vertical))
                AddVerticalLayout(spacing, childAlignment, childControlWidth, childControlHeight, childForceExpandWidth,
                    childForceExpandHeight, reverseArrangement);
            if (direction.HasFlag(ScrollViewDirection.Horizontal))
                AddHorizontalLayout(spacing, childAlignment, childControlWidth, childControlHeight, childForceExpandWidth,
                    childForceExpandHeight, reverseArrangement);

            return this;
        }

        public BaseComponent AddHorizontalLayout(float spacing = 0f, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false) 
        {
            HorizontalLayout = CreateLayout<HorizontalLayoutGroup>(gameObject, spacing, childAlignment, childControlWidth, childControlHeight, childForceExpandWidth, childForceExpandHeight, reverseArrangement);
            return this;
        }
        public BaseComponent AddVerticalLayout(float spacing = 0f, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false) 
        {
            VerticalLayout = CreateLayout<VerticalLayoutGroup>(gameObject, spacing, childAlignment, childControlWidth, childControlHeight, childForceExpandWidth, childForceExpandHeight, reverseArrangement);
            return this;
        }

        public BaseComponent AddFitter(ScrollViewDirection dir, ContentSizeFitter.FitMode fitMode = ContentSizeFitter.FitMode.PreferredSize)
        {
            ContentSizeFitter = gameObject.GetOrAddComponent<ContentSizeFitter>();
            ContentSizeFitter.verticalFit = dir.HasFlag(ScrollViewDirection.Vertical) ? fitMode : ContentSizeFitter.FitMode.Unconstrained;
            ContentSizeFitter.horizontalFit = dir.HasFlag(ScrollViewDirection.Horizontal) ? fitMode : ContentSizeFitter.FitMode.Unconstrained;
            return this;
        }

        public BaseComponent Padding(RectOffset padding, ScrollViewDirection direction)
        {
            if (direction.HasFlag(ScrollViewDirection.Vertical))
            {
                VerticalLayout.padding = padding;
            }
            if (direction.HasFlag(ScrollViewDirection.Horizontal))
            {
                HorizontalLayout.padding = padding;
            }
            return this;
        }
        public BaseComponent Padding(int amount, ScrollViewDirection direction)
        {
            return Padding(PaddingSide.All, amount, direction);
        }
        public BaseComponent Padding(PaddingSide side, int amount, ScrollViewDirection direction)
        {
            if (side.HasFlag(PaddingSide.Leading)) { if (HorizontalLayout && direction.HasFlag(ScrollViewDirection.Horizontal)) HorizontalLayout.padding.left = amount; if (VerticalLayout&& direction.HasFlag(ScrollViewDirection.Vertical)) VerticalLayout.padding.left = amount;}
            if (side.HasFlag(PaddingSide.Trailing)) { if (HorizontalLayout&& direction.HasFlag(ScrollViewDirection.Horizontal)) HorizontalLayout.padding.right = amount;  if (VerticalLayout&& direction.HasFlag(ScrollViewDirection.Vertical)) VerticalLayout.padding.right = amount;}
            if (side.HasFlag(PaddingSide.Top)) { if (HorizontalLayout&& direction.HasFlag(ScrollViewDirection.Horizontal)) HorizontalLayout.padding.top = amount;  if (VerticalLayout&& direction.HasFlag(ScrollViewDirection.Vertical)) VerticalLayout.padding.top= amount;}
            if (side.HasFlag(PaddingSide.Bottom)) { if (HorizontalLayout&& direction.HasFlag(ScrollViewDirection.Horizontal)) HorizontalLayout.padding.bottom = amount;  if (VerticalLayout&& direction.HasFlag(ScrollViewDirection.Vertical)) VerticalLayout.padding.bottom = amount;}
            return this;
        }

        public BaseComponent Copy(bool fullyCopyRect = true)
        {
            BaseComponent copyComponent = this.BaseCopy(this);
            return copyComponent.CopyFrom(this, fullyCopyRect);
        }

        public virtual BaseComponent CopyFrom(BaseComponent other, bool fullyCopyRect = true)
        {
            if (!other)
            {
                UCGUILogger.LogError("<b>Cannot copy from null!</b>");
                return this;
            }
            CopyRect(other.GetRect(), this, fullyCopyRect);
            CopyLayoutElement(other, this);
            CopyLayouts(other, this);
            return this;
        }
    }
}