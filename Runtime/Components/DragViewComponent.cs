using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's DragViewComponent. Inherits from ViewComponment but can be dragged around.
    /// Optionally limited within a confined space.
    /// <br></br>
    /// <br></br>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="Bounds"/> - Specify an area in which the view can move inside. Optionally specify an offset to allow for padding / movement of the area.</description></item>
    /// </list>
    /// </summary>
    public partial class DragViewComponent : ViewComponent, IDragHandler
    {
        protected Rect boundArea;
        protected RectOffset boundsOffset;
        public bool HasBounds => boundArea.size.magnitude > 0;
        
        public override void Awake()
        {
            base.Awake();
            this.Pivot(PivotPosition.UpperLeft);
        }

        public override void Start()
        {
            base.Start();
            this.Pos(-this.GetWidth() / 2, this.GetHeight() / 2);
            DisplayName = "DragViewComponent";
        }
        
        /// <summary>
        /// Creates an invisible bounding box for the view to move in.
        /// </summary>
        /// <param name="bounds">The size of the bounds.</param>
        /// <param name="offset">The offset describing the padding inside the base bounds.</param>
        public virtual DragViewComponent Bounds(Vector2 bounds, RectOffset offset = null)
        {
            boundArea = new Rect(new Vector2(0, 0), bounds);
            
            if (offset != null)
                boundsOffset = offset;
            else
                boundsOffset ??= new RectOffset(0, 0, 0, 0);
            
            return this;
        }

        public virtual DragViewComponent BoundsOffset(RectOffset offset)
        {
            boundsOffset = offset;
            return this;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (IsLocked)
                return;
            
            Vector2 newPos = this.GetPos() + eventData.delta;
            if (!HasBounds)
            {
                this.Pos(newPos);
                return;
            }

            float boundsWidth = boundArea.width / 2;
            float boundsHeight = boundArea.height / 2;
            float rightBorder = boundsWidth - this.GetWidth() - boundsOffset.right;
            float leftBorder = -boundsWidth + boundsOffset.left;
            float topBorder = boundsHeight - boundsOffset.top;
            float bottomBorder = -boundsHeight + this.GetHeight() + boundsOffset.bottom;
            
            if (newPos.x < leftBorder)
                newPos.x = leftBorder;
            else if (newPos.x > rightBorder)
                newPos.x = rightBorder;
            
            if (newPos.y < bottomBorder)
                newPos.y = bottomBorder;
            else if (newPos.y > topBorder)
                newPos.y = topBorder;
            
            this.Pos(newPos);
        }
        
        public class DragViewBuilder : ViewBuilder
        {
            private DragViewComponent _dragView;
            public DragViewBuilder(DragViewComponent dragViewComponent, Canvas canvas = null) : base(dragViewComponent, canvas)
            {
                _dragView = dragViewComponent;
            }

            /// <summary>
            /// Sets the size and position of the bounds within dragging is allowed.
            /// If the bounds are not set, the view can be dragged everywhere.
            /// </summary>
            /// <param name="bounds"></param>
            /// <param name="boundsOffset"></param>
            public void Bounds(Vector2 bounds, RectOffset boundsOffset = null) => _dragView.Bounds(bounds, boundsOffset);
        }
    }
}