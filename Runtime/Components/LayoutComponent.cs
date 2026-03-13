using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public abstract class LayoutComponent : ImageComponent
    {
        protected LayoutComponent() {}
        
        protected override void Awake()
        {
            base.Awake();
            Color(UnityEngine.Color.clear);
        }

        public virtual void Add(params BaseComponent[] elements) 
        {
            foreach (var element in elements)
            {
                element.Parent(this);
            }
        }

        public override void Enabled(bool on)
        {
            base.Enabled(on);
            foreach (Transform child in GetRect())
            {
                if (child.GetComponent<MonoBehaviour>() is IEnabled e)
                {
                    e.Enabled(on);
                }
            }
        }

        public override BaseComponent HandleSizeChanged(Vector2 old, Vector2 updated)
        {
            if (ContentSizeFitter)
            {
                if (!updated.x.Equals(old.x))
                    ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                if (!updated.y.Equals(old.y))
                    ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            }
            return base.HandleSizeChanged(old, updated);
        }

        /// <summary>
        /// Marks this element for rebuild inside the Canvas. 
        /// </summary>
        public void MarkForRebuild()
        {
            LayoutRebuilder.MarkLayoutForRebuild(GetRect());
        }

        /// <summary>
        /// Force rebuilds this element based on its current position in the hierarchy.
        /// </summary>
        /// <remarks>WARNING: Should only be used with care as it can be resource intensive and cause lag.</remarks>
        public void ForceRebuild()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetRect());
        }

        /// <summary>
        /// Calls <see cref="SpacerComponent.RenderImmediate"/> or <see cref="SpacerComponent.Render"/> on all containing <see cref="SpacerComponent"/>s.
        /// </summary>
        /// <param name="immediate">Whether to immediately re-render the spacer or not.</param>
        public void RerenderSpacers(bool immediate = true)
        {
            foreach (RectTransform child in GetRect().transform)
            {
                var spacer = child.gameObject.GetComponent<SpacerComponent>();
                if (spacer)
                {
                    if (immediate)
                        spacer.RenderImmediate();
                    else
                        spacer.Render();
                }
            }
        }
    }
}