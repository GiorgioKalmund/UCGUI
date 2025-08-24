using System.Collections;
using UCGUI.Services;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class BaseComponent : MonoBehaviour, ICopyable<BaseComponent>
    {
        private RectTransform _rect;
        public bool paddingApplied = false;
        public bool posApplied = false;

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


        public LayoutElement layoutElement;
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
                Debug.LogWarning(gameObject.name  +": RectTransform is null! Did you forget to call 'base.Awake()' somewhere?");
                return gameObject.GetComponent<RectTransform>();
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
            layoutElement = gameObject.GetOrAddComponent<LayoutElement>();
            return layoutElement;
        }

        public LayoutElement AddLayoutElement(float minWidth, float minHeight)
        {
            AddLayoutElement();
            MinimumSize(minWidth, minHeight);
            return layoutElement;
        }
        
        
        public BaseComponent MinimumSize(Vector2 size)
        {
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
        
        public void CopyLayoutElement(BaseComponent other, BaseComponent component)
        {
            LayoutElement layoutElement = other.GetComponent<LayoutElement>();
            if (layoutElement)
            {
                var newLayoutElement = component.gameObject.GetOrAddComponent<LayoutElement>();
                newLayoutElement.preferredWidth = layoutElement.preferredWidth;
                newLayoutElement.preferredHeight = layoutElement.preferredHeight;
                newLayoutElement.minWidth = layoutElement.minWidth;
                newLayoutElement.minHeight= layoutElement.minHeight;
                newLayoutElement.layoutPriority = layoutElement.layoutPriority;
            }
        }
        
        protected T AddLayout<T>(GameObject obj, float spacing, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false) where T : HorizontalOrVerticalLayoutGroup
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

        public BaseComponent AddHorizontalLayout(float spacing = 0f, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false) 
        {
            HorizontalLayout = AddLayout<HorizontalLayoutGroup>(gameObject, spacing, childAlignment, childControlWidth, childControlHeight, childForceExpandWidth, childForceExpandHeight, reverseArrangement);
            return this;
        }
        public BaseComponent AddVerticalLayout(float spacing = 0f, TextAnchor childAlignment = TextAnchor.MiddleCenter, bool childControlWidth = false, bool childControlHeight = false, bool childForceExpandWidth = false, bool childForceExpandHeight = false, bool reverseArrangement = false) 
        {
            VerticalLayout = AddLayout<VerticalLayoutGroup>(gameObject, spacing, childAlignment, childControlWidth, childControlHeight, childForceExpandWidth, childForceExpandHeight, reverseArrangement);
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
            CopyRect(other.GetRect(), this, fullyCopyRect);
            CopyLayoutElement(other, this);
            CopyLayouts(other, this);
            return this;
        }
    }
}