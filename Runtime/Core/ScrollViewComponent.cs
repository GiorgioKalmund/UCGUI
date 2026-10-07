using System;
using System.Collections.Generic;
using UCGUI.Styles;
using UnityEngine;
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
    /// </summary>
    public class ScrollViewComponent : ImageComponent, IStylable<ScrollViewComponent, ScrollViewStyle>
    {
        protected ScrollViewComponent() {}

        protected BaseComponent  viewport;
        protected internal SwitchLayoutComponent content;

        public UIBehaviour mask;
        public ScrollRect scrollRect;

        protected ImageComponent scrollbarVertical;
        protected ImageComponent scrollbarVerticalHandle;
        protected ImageComponent scrollbarHorizontal;
        protected ImageComponent scrollbarHorizontalHandle;

        protected TextAnchor contentLayoutDirectionVertical;
        protected TextAnchor contentLayoutDirectionHorizontal;

        public List<InputAction> disableWhileHovering = new List<InputAction>();

        protected override void Awake()
        {
            base.Awake();

            viewport = UI.Blank().Parent(transform)
                    .DisplayName("Viewport")
                    .Maximize();
            
            // Add RectMask2D by default and no dedicated mask image
            mask = viewport.gameObject.AddComponent<RectMask2D>();

            content = UI.N<SwitchLayoutComponent>().Parent(viewport)
                    .DisplayName("Content");

            contentLayoutDirectionVertical = Defaults.Layout.StandardVerticalChildAlignment;
            contentLayoutDirectionHorizontal = Defaults.Layout.StandardHorizontalChildAlignment;

            // Set up scroll rect (values taken from default uGUI ScrollView instance)
            scrollRect = gameObject.AddComponent<ScrollRect>();
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
                scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
                scrollRect.decelerationRate = 0.135f;
            scrollRect.scrollSensitivity = 1.0f;
            
            scrollRect.viewport = viewport.GetRect();
            scrollRect.content = content.GetRect();
            
            viewport.Pivot(PivotPosition.UpperLeft, true);

            scrollRect.horizontalScrollbar = CreateScrollbar(Defaults.ScrollView.HorizontalScrollbarPlacement, Defaults.ScrollView.HorizontalScrollbarHeight, out scrollbarHorizontal, out scrollbarVerticalHandle);
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = 0;
            
            scrollRect.verticalScrollbar = CreateScrollbar(Defaults.ScrollView.VerticalScrollbarPlacement, Defaults.ScrollView.VerticalScrollbarWidth, out scrollbarVertical, out scrollbarHorizontalHandle);
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarSpacing = 0;

            Style(ScrollViewStyle.Plain);
            StyleScrollbarVertical(ScrollViewStyle.PlainBar, ScrollViewStyle.PlainHandle);
            StyleScrollbarHorizontal(ScrollViewStyle.PlainBar, ScrollViewStyle.PlainHandle);
        }

        /// <summary>
        /// Adds safe space to the inner <see cref="content"/>.
        /// </summary>
        /// <param name="safeSpace">The <see cref="RectOffset"/> defining the safe space of the <see cref="content"/>.</param>
        public ScrollViewComponent ContentSafeSpace(RectOffset safeSpace)
        {
            content.Padding(safeSpace);
            return this;
        }

        /// <summary>
        /// Adds safe space to the inner <see cref="content"/>.
        /// </summary>
        /// <param name="side">The side to add the safe space to.</param>
        /// <param name="amount">The amount of safe space to add.</param>
        public ScrollViewComponent ContentSafeSpace(PaddingSide side, int amount)
        {
            content.Padding(side, amount);
            return this;
        }

        public ScrollViewComponent ContentSpacing(float spacing)
        {
            content.Spacing(spacing);
            return this;
        }
        
        public ScrollViewComponent ContentChildAlignment(TextAnchor childAlignmentV, TextAnchor childAlignmentH)
        {
            contentLayoutDirectionVertical = childAlignmentV;
            contentLayoutDirectionHorizontal = childAlignmentH;
            if (content.IsVertical())
                content.ChildAlignment(childAlignmentV);
            else
                content.ChildAlignment(childAlignmentH);
            return this;
        }

        public ScrollViewComponent ContentReverseArrangement(bool reverse = true)
        {
            content.ReverseArrangement(reverse);
            return this;
        }

        /// <summary>
        /// The inset of the viewport's content using a mask.
        /// </summary>
        /// <param name="amount">The inset amount described by the members of the <see cref="RectOffset"/>.</param>
        /// <returns></returns>
        public ScrollViewComponent ViewportInset(RectOffset amount)
        {
            if (mask is RectMask2D mask2D)
            {
                var pad = mask2D.padding;
                pad.x = amount.left;       // x = left
                pad.y = amount.bottom;     // y = bottom
                pad.z = amount.right;      // z = right
                pad.w = amount.top;        // w = top
                mask2D.padding = pad;
            }
            else
            {
                UCGUILogger.LogWarning("The current mask does not support this type of viewport insetting! Please use a RectMask2D (default).");
            }

            return this;
        }
        
        /// <summary>
        /// The inset of the viewport's content using a mask.
        /// </summary>
        /// <param name="side">The side to add the inset to.</param>
        /// <param name="amount">The amount of inset to add.</param>
        /// <returns></returns>
        public ScrollViewComponent ViewportInset(PaddingSide side, int amount)
        {
            if (mask is RectMask2D mask2D)
            {
                var padding = mask2D.padding;
                if (side.HasFlag(PaddingSide.Leading))
                    padding.x = amount;
                if (side.HasFlag(PaddingSide.Bottom))
                    padding.y = amount;
                if (side.HasFlag(PaddingSide.Trailing))
                    padding.z = amount;
                if (side.HasFlag(PaddingSide.Top))
                    padding.w = amount;
                mask2D.padding = padding;
            }
            else
            {
                UCGUILogger.LogWarning("The current mask does not support this type of viewport insetting! Please use a RectMask2D (default).");
            }
            return this;
        }


        public void ScrollToStart()
        {
            if (scrollRect.vertical && !scrollRect.horizontal)
                content.Pos(content.GetPos().x, 0);  
            else if (scrollRect.horizontal && !scrollRect.vertical)
                content.Pos(0, content.GetPos().y);  
            else
                UCGUILogger.LogWarning($"ScrollView cannot scroll to start, as the current scrolling direction setup has no clear start ('{((scrollRect.vertical && scrollRect.horizontal) ? ScrollViewDirection.Both : ScrollViewDirection.None)}').", this);
            
        }

        public void ScrollToChild(RectTransform child)
        {
            // TODO: @Incomplete
        }
        
        public void ScrollToEnd()
        {
            if (scrollRect.vertical && !scrollRect.horizontal)
            {
                if (content.GetHeight() < this.GetHeight()) return;
                content.Pos(content.GetPos().x, content.GetHeight() -  this.GetHeight());  
            }
            else if (scrollRect.horizontal && !scrollRect.vertical)
            {
                if (content.GetWidth() < this.GetWidth()) return;
                content.Pos(content.GetWidth() -  this.GetWidth(), content.GetPos().y);  
            }
            else
                UCGUILogger.LogWarning($"ScrollView cannot scroll to start, as the current scrolling direction setup has no clear start ('{((scrollRect.vertical && scrollRect.horizontal) ? ScrollViewDirection.Both : ScrollViewDirection.None)}').", this);
        } 

        /// <summary>
        /// Sets the size of the viewport, <b>including</b> any possible scrollbars.
        /// </summary>
        /// <param name="size">The size of the viewport.</param>
        public ScrollViewComponent ViewportSize(Vector2 size) => this.Size(size);

        // TODO @Cleanup: Maybe we can warn the user when they call this even though we are already in this direction and / or layout (i.e. duplicate calls)
        public ScrollViewComponent ScrollingDirection(ScrollViewDirection dir, bool alsoChangeLayoutDirection = false)
        {
            if (alsoChangeLayoutDirection) 
                LayoutDirection(dir);
            
            scrollRect.vertical = dir.HasFlag(ScrollViewDirection.Vertical);
            scrollRect.horizontal = dir.HasFlag(ScrollViewDirection.Horizontal);
            
            content.AddFitter(dir, ContentSizeFitter.FitMode.PreferredSize);

            if (scrollRect.vertical && !scrollRect.horizontal)
                content.StretchHorizontally(HorizontalStretchAnchor.Top);
            else if (scrollRect.horizontal && !scrollRect.vertical)
                content.StretchVertically(VerticalStretchAnchor.Left);
            
            return this;
        }

        public ScrollViewComponent LayoutDirection(ScrollViewDirection dir)
        {
            if (dir.Equals(ScrollViewDirection.Both))
            {
                //UCGUILogger.LogWarning($"Could not switch content layout direction of scroll view '{gameObject.name}' to '{ScrollViewDirection.Both}'. This is not supported. Only the scrolling direction supports '{ScrollViewDirection.Both}'! Layout falling back to '{ScrollViewDirection.Vertical}'", this);
                dir = content.IsVertical() ? ScrollViewDirection.Vertical: ScrollViewDirection.Horizontal; // Default to laying out the objects in the original direction of the content.
            }
            
            if (dir.Equals(ScrollViewDirection.Vertical) && !content.IsVertical())
                content.MakeVertical().ChildAlignment(contentLayoutDirectionVertical);
            else if (dir.Equals(ScrollViewDirection.Horizontal) && !content.IsHorizontal())
                content.MakeHorizontal().ChildAlignment(contentLayoutDirectionHorizontal);
            else if (dir.Equals(ScrollViewDirection.None) && !content.IsVertical() && !content.IsHorizontal())
                content.MakeDirection(ScrollViewDirection.None);
            
            
            return this;
        }

        protected Scrollbar CreateScrollbar(PaddingSide side, int size, out ImageComponent scrollbar, out ImageComponent handle)
        {
            scrollbar = UI.Image().DisplayName($"Scrollbar {side}");
            Scrollbar bar = scrollbar.gameObject.AddComponent<Scrollbar>();
            BaseComponent slidingArea = UI.Blank().Parent(scrollbar).DisplayName("Sliding Area");
            handle = UI.Image().Parent(slidingArea).DisplayName("Handle");
            
            int half = size / 2;
            slidingArea
                .Maximize()
                .RectOffsets(new RectOffset(half, half, half, half));

            if (PaddingSide.Horizontal.HasFlag(side))
            {
                bar.direction = Scrollbar.Direction.BottomToTop;
                handle.StretchHorizontally(HorizontalStretchAnchor.Middle);
                
                scrollbar.Width(size);
            } else if (PaddingSide.Vertical.HasFlag(side))
            {

                bar.direction = Scrollbar.Direction.LeftToRight;
                handle.Maximize();
                
                scrollbar.Height(size);
            }
            
            ScrollbarPosition(side, scrollbar);
            
            handle
                .RectOffsets(new RectOffset(-half, -half, -half, -half));

            bar.handleRect = handle.GetRect();

            scrollbar.Parent(this);
            
            return bar;
        }

        /*
         * TODO @Cleanup: Again, we allow too many options which might lead to confusion (All, Horizontal, Vertical)
         * This could also allow us to compact some of the XXXVertical and XXXHorizontal functions into one function
         */
        /// <summary>
        /// Positions the appropriate scrollbar object at the given side.
        /// If none is supplied, it will automatically pick the correct one from the existing two.
        /// </summary>
        /// <param name="side">The side to position the scrollbar at.</param>
        /// <param name="scrollbar">Optional reference to a scrollbar object to move, instead of picking the appropriate one.</param>
        /// <remarks>
        /// This means that <see cref="PaddingSide.Leading"/> and <see cref="PaddingSide.Trailing"/> only affect the vertical scrollbar
        /// and <see cref="PaddingSide.Top"/> and <see cref="PaddingSide.Bottom"/> only affect the horizontal scrollbar.
        /// </remarks>
        public ScrollViewComponent ScrollbarPosition(PaddingSide side, ImageComponent scrollbar = null)
        {
            if (PaddingSide.Horizontal.HasFlag(side))
            {
                scrollbar ??= scrollbarVertical;
                scrollbar.StretchVertically(side.Equals(PaddingSide.Leading) ? VerticalStretchAnchor.Left : VerticalStretchAnchor.Right);
                var p = viewport.GetRect().pivot;
                p.x = side.Equals(PaddingSide.Trailing) ? 0 : 1;
                viewport.Pivot(p);
            }
            else
            {
                scrollbar ??= scrollbarHorizontal;
                scrollbar.StretchHorizontally(side.Equals(PaddingSide.Top) ? HorizontalStretchAnchor.Top: HorizontalStretchAnchor.Bottom);
                var p = viewport.GetRect().pivot;
                p.y = side.Equals(PaddingSide.Top) ? 0 : 1;
                viewport.Pivot(p);
            }

            return this;
        }

        public ScrollViewComponent ScrollbarVerticalWidth(int verticalScrollbarWidth)
        {
            scrollbarVertical.Width(verticalScrollbarWidth);
            return this;
        }
        
        public ScrollViewComponent ScrollbarHorizontalHeight(int horizontalScrollbarHeight)
        {
            scrollbarVertical.Width(horizontalScrollbarHeight);
            return this;
        }

        public ScrollViewComponent ScrollbarVerticalVisibility(ScrollRect.ScrollbarVisibility visibility)
        {
            scrollRect.verticalScrollbarVisibility = visibility;
            return this;
        }
        
        public ScrollViewComponent ScrollbarHorizontalVisibility(ScrollRect.ScrollbarVisibility visibility)
        {
            scrollRect.horizontalScrollbarVisibility = visibility;
            return this;
        }

        /// <summary>
        /// Fully enables and attaches, or disables and detaches a scrollbar. 
        /// </summary>
        /// <returns>The GameObject of the vertical scrollbar.</returns>
        public GameObject ScrollbarVerticalEnabled(bool e)
        {
            scrollRect.verticalScrollbar = e ? scrollbarVertical.GetComponent<Scrollbar>() : null;
            return scrollbarVertical.SetActive(e).gameObject;
        }
           

        /// <summary>
        /// Fully enables and attaches, or disables and detaches a scrollbar. 
        /// </summary>
        /// <returns>The GameObject of the vertical scrollbar.</returns>
        public GameObject ScrollbarHorizontalEnabled(bool e)
        {
            scrollRect.horizontalScrollbar = e ? scrollbarHorizontal.GetComponent<Scrollbar>() : null;
            return scrollbarHorizontal.SetActive(e).gameObject;
        } 
        
        public ScrollViewComponent ScrollbarVerticalSpacing(float spacing)
        {
            scrollRect.verticalScrollbarSpacing = spacing;
            return this;
        }
        
        public ScrollViewComponent ScrollbarHorizontalSpacing(float spacing)
        {
            scrollRect.horizontalScrollbarSpacing = spacing;
            return this;
        }

        public ScrollViewComponent MovementType(ScrollRect.MovementType movementType)
        {
            scrollRect.movementType = movementType;
            return this;
        }

        // Idea: Disable certain controls when hovering over scroll views, to avoid scrolling in other areas as well 
        public override void HandlePointerEnter(PointerEventData eventData)
        {
            foreach (var inputAction in disableWhileHovering)
            {
                inputAction.Disable();
            }
        }

        public override void HandlePointerExit(PointerEventData eventData)
        {
            foreach (var inputAction in disableWhileHovering)
            {
                inputAction.Enable();
            }
        }
        
        public override void Enabled(bool e)
        {
            base.Enabled(e);
            scrollRect.enabled = e;
            content.SetActive(e);
            ScrollbarVerticalEnabled(e);
            ScrollbarHorizontalEnabled(e);
        }

        public ScrollViewComponent Style(ScrollViewStyle style)
        {
            style.Apply(this);
            return this;
        }

        public ScrollViewComponent StyleScrollbarVertical(ImageStyle barStyle, ImageStyle handleStyle)
        {
            barStyle.Apply(scrollbarVertical);
            handleStyle.Apply(scrollbarVerticalHandle);
            return this;
        }
        
        
        public ScrollViewComponent StyleScrollbarHorizontal(ImageStyle barStyle, ImageStyle handleStyle)
        {
            barStyle.Apply(scrollbarHorizontal);
            handleStyle.Apply(scrollbarHorizontalHandle);
            return this;
        }
    }
}