using UnityEngine;

namespace UCGUI
{
    public abstract class LayoutComponent : ImageComponent
    {
        public override void Awake()
        {
            base.Awake();
            ToggleVisibility();
        }
        
        public virtual void Add(BaseComponent element)
        {
            element.Parent(this);
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
    }
}