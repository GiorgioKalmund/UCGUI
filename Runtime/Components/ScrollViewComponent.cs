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
    
    public class ScrollViewComponent : ImageComponent
    {
        public ImageComponent content;

        private RectMask2D _mask;
        private ScrollRect _scroll;

        private GridLayoutGroup _grid;

        public static List<InputAction> DisableWhileHovering = new List<InputAction>();

        public override void Awake()
        {
            base.Awake();

            _scroll = gameObject.AddComponent<ScrollRect>();
            _mask = gameObject.AddComponent<RectMask2D>();

            content = ComponentExtension.N<ImageComponent>("Content", transform)
                    .Alpha(0f)
                ;

            _scroll.content = content.GetRect();

            _scroll.movementType = ScrollRect.MovementType.Clamped;
        }

        public override void Start()
        {
            base.Start();
            DisplayName = "ScrollViewComponent";
        }

        public ScrollViewComponent ContentPadding(PaddingSide side, int amount)
        {
            base.Padding(side, amount, ScrollViewDirection.Both);
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

        public ScrollViewComponent AddContent(BaseComponent component)
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

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);
            if (content)
                content.Size(x, y);
            return this;
        }
        
        public ScrollViewComponent ScrollingDirection(ScrollViewDirection dir)
        {
            _scroll.vertical = dir.HasFlag(ScrollViewDirection.Vertical);
            _scroll.horizontal = dir.HasFlag(ScrollViewDirection.Horizontal);
            return this;
        }

        public ScrollViewComponent MovementType(ScrollRect.MovementType movementType)
        {
            _scroll.movementType = movementType;
            return this;
        }

        // -- Idea: Disable certain controls when hovering over scroll views, to avoid scrolling in other areas as well -- //
        public override void HandlePointerEnter(PointerEventData eventData)
        {
            foreach (var inputAction in DisableWhileHovering)
            {
                inputAction.Disable();
            }
        }

        public override void HandlePointerExit(PointerEventData eventData)
        {
            foreach (var inputAction in DisableWhileHovering)
            {
                inputAction.Enable();
            }
        }

        public ContentSizeFitter GetFitter()
        {
            return ContentSizeFitter;
        }

        public ScrollViewComponent Enabled(bool e)
        {
            _scroll.enabled = e;
            return this;
        }
    }
}