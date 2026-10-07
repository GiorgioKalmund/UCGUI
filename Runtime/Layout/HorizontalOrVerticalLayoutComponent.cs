using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public abstract class HorizontalOrVerticalLayoutComponent : LayoutComponent 
    {
        protected override void Awake()
        {
            base.Awake();
            AddFitter(ScrollViewDirection.Both, ContentSizeFitter.FitMode.PreferredSize);
        }

        protected virtual void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetRect());
        }

        protected abstract HorizontalOrVerticalLayoutGroup GetLayout();

        public HorizontalOrVerticalLayoutComponent Spacing(float spacing)
        {
            if (GetLayout())
                GetLayout().spacing = spacing;
            else
                UCGUILogger.LogWarning("No layout present!", this);
            return this;
        }

        public HorizontalOrVerticalLayoutComponent ChildAlignment(TextAnchor childAlignment)
        {
            if (GetLayout())
                GetLayout().childAlignment = childAlignment;
            else
                UCGUILogger.LogWarning("No layout present!", this);
            return this;
        }

        public HorizontalOrVerticalLayoutComponent ReverseArrangement(bool reverse = true)
        {
            if (GetLayout())
                GetLayout().reverseArrangement = reverse;
            else
                UCGUILogger.LogWarning("No layout present!", this);
            return this;
        }
        
        public HorizontalOrVerticalLayoutComponent ReverseArrangementToggle()
        {
            if (GetLayout())
                GetLayout().reverseArrangement = !GetLayout().reverseArrangement;
            else
                UCGUILogger.LogWarning("No layout present!", this);
            return this;
        }

        public bool IsHorizontal()
        {
            return HorizontalLayout != null;
        }
        
        public bool IsVertical()
        {
            return VerticalLayout != null;
        }
    }
}