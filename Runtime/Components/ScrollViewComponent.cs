using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UCGUI
{
    [Flags]
    public enum ScrollViewDirection
    {
        Horizontal = 1,
        Vertical = 2,
        Both = Horizontal | Vertical,
        None = 0
        
    }
    
    /// <summary>
    /// UCGUI's default Scroll View Component.
    /// Noteworthy formatting functions:
    /// <list type="bullet">
    /// <item><description><see cref="ScrollToBottom"/> - Scrolls to the bottom of the scroll view.</description></item>
    /// <item><description><see cref="ScrollToTop"/> - Scrolls to the top of the scroll view.</description></item>
    /// <item><description><see cref="ScrollViewDirection"/> - Specifies the <see cref="ScrollViewDirection"/>(s) in which the scroll view should scroll.</description></item>
    /// <item><description><see cref="Add(BaseComponent)"/> - Adds specified content to the scroll view as a child.</description></item>
    /// <item><description><see cref="ContentPadding"/> - Specifies the inner padding to be applied within the bounds of the scroll view.</description></item>
    /// <item><description><see cref="disableWhileHovering"/> - A list containing all other input actions which are to be disabled when hovering over the scroll view.</description></item>
    /// </list>
    /// </summary>
    public partial class ScrollViewComponent : BaseComponent, IPointerEnterHandler, IPointerExitHandler, IEnabled
    {
        protected ScrollViewComponent() {}
        
        public ImageComponent content;

        public RectMask2D mask;
        public ScrollRect scrollRect;

        public List<InputAction> disableWhileHovering = new List<InputAction>();

        public override void Awake()
        {
            base.Awake();
            DisplayName = "ScrollViewComponent";

            scrollRect = gameObject.AddComponent<ScrollRect>();
            mask = gameObject.AddComponent<RectMask2D>();

            content = UI.N<ImageComponent>(transform)
                    .DisplayName("Content")
                    .Alpha(0)
                ;

            scrollRect.content = content.GetRect();

            // By default, use Clamped
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

        public ScrollViewComponent ContentPadding(PaddingSide side, int amount)
        {
            Padding(side, amount);
            return this;
        }

        public void ScrollToBottom()
        {
            content.Pos(content.GetPos().x, (content.GetHeight() -  this.GetHeight()) / 2);
        }
        
        public void ScrollToTop()
        {
            content.Pos(content.GetPos().x, -(content.GetHeight() -  this.GetHeight()) / 2);
        }

        public ScrollViewComponent Add(BaseComponent component)
        {
            component.Parent(content);

            // Maybe there is a more elegant, less computationally heavy solution, but this works for now
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetRect());

            if (VerticalLayout)
            {
                VerticalLayout.CalculateLayoutInputVertical();
                VerticalLayout.SetLayoutVertical();
            }
            
            return this;
        }
        
        public ScrollViewComponent Add(params BaseComponent[] components)
        {
            foreach (var component in components)
            {
                Add(component);
            }

            return this;
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);
            if (content)
                content.Size(x, y);
            return this;
        }
        
        public ScrollViewComponent ScrollingDirection(ScrollViewDirection dir)
        {
            scrollRect.vertical = dir.HasFlag(ScrollViewDirection.Vertical);
            scrollRect.horizontal = dir.HasFlag(ScrollViewDirection.Horizontal);
            return this;
        }

        public ScrollViewComponent MovementType(ScrollRect.MovementType movementType)
        {
            scrollRect.movementType = movementType;
            return this;
        }

        // -- Idea: Disable certain controls when hovering over scroll views, to avoid scrolling in other areas as well -- //
        public virtual void HandlePointerEnter(PointerEventData eventData)
        {
            foreach (var inputAction in disableWhileHovering)
            {
                inputAction.Disable();
            }
        }

        public virtual void HandlePointerExit(PointerEventData eventData)
        {
            foreach (var inputAction in disableWhileHovering)
            {
                inputAction.Enable();
            }
        }
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            HandlePointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandlePointerExit(eventData);
        }

        public void Enabled(bool e)
        {
            scrollRect.enabled = e;
        }

        public partial class ScrollViewBuilder
        {
            private readonly ScrollViewComponent _scrollView;
            public ScrollViewBuilder(ScrollViewComponent scrollViewComponent) => _scrollView = scrollViewComponent;
            public void Add(params BaseComponent[] content) => _scrollView.Add(content);
        }
    }
}