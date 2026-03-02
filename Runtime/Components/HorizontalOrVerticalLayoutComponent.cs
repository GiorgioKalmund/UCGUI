using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public abstract class HorizontalOrVerticalLayoutComponent : LayoutComponent 
    {
        protected HorizontalOrVerticalLayoutComponent() {}
        
        protected override void Awake()
        {
            base.Awake();
            AddFitter(ScrollViewDirection.Both);
        }

        protected virtual void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetRect());
        }

        protected abstract HorizontalOrVerticalLayoutGroup GetLayout();

        public HorizontalOrVerticalLayoutComponent Spacing(float spacing)
        {
            GetLayout().spacing = spacing;
            return this;
        }

        public HorizontalOrVerticalLayoutComponent ChildAlignment(TextAnchor childAlignment)
        {
            GetLayout().childAlignment = childAlignment;
            return this;
        }

        public HorizontalOrVerticalLayoutComponent ReverseArrangement(bool reverse = true)
        {
            GetLayout().ReverseArrangement(reverse);
            return this;
        }
        
        public HorizontalOrVerticalLayoutComponent ReverseArrangementToggle()
        {
            GetLayout().ReverseArrangement(!GetLayout().reverseArrangement);
            return this;
        }
    }
}