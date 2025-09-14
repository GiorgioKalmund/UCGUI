using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public abstract class HorizontalOrVerticalLayoutComponent : LayoutComponent 
    {
        public override void Awake()
        {
            base.Awake();
            AddFitter(ScrollViewDirection.Both);
        }

        protected abstract HorizontalOrVerticalLayoutGroup GetLayout();

        public HorizontalOrVerticalLayoutComponent Spacing(float spacing)
        {
            GetLayout().spacing = spacing;
            return this;
        }

        public HorizontalOrVerticalLayoutComponent Padding(RectOffset padding)
        {
            return this;
        }

        public HorizontalOrVerticalLayoutComponent Padding(PaddingSide side, int amount)
        {
            GetLayout().Padding(side, amount);
            return this;
        }

        public HorizontalOrVerticalLayoutComponent PaddingAdd(PaddingSide side, int amount)
        {
            GetLayout().PaddingAdd(side, amount);
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
    }
}