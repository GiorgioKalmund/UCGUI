using UnityEngine;

namespace UCGUI
{
    using UnityEngine.UI;
    public partial class LayoutBuilder
    {
        private HorizontalOrVerticalLayoutGroup _layoutGroup;

        public LayoutBuilder(HorizontalOrVerticalLayoutGroup layoutGroup) { _layoutGroup = layoutGroup; }

        public void Padding(RectOffset padding)
        {
            _layoutGroup.padding = padding;
        }

        public void ReverseArrangement(bool reverse = true)
        {
            _layoutGroup.reverseArrangement = reverse;
        }
        
        public virtual void Add(params BaseComponent[] component)
        {
            foreach (var baseComponent in component)
            {
                baseComponent.Parent(_layoutGroup.GetTransform());
            }
        }
    }
}