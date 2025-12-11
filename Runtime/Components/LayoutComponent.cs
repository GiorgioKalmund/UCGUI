using UnityEngine;

namespace UCGUI
{
    public abstract class LayoutComponent : ImageComponent
    {
        protected LayoutComponent() {}
        
        public override void Awake()
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

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            if (ContentSizeFitter)
                ContentSizeFitter.enabled = false;
            base.HandleSizeChanged(x, y);
            return this;
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