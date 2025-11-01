using UnityEngine;

namespace UCGUI
{
    using UnityEngine.UI;
    public partial class LayoutBuilder
    {
        private HorizontalOrVerticalLayoutComponent _layout;
        private HorizontalOrVerticalLayoutGroup _relevantLayout;

        public LayoutBuilder(HorizontalOrVerticalLayoutComponent layout, HorizontalOrVerticalLayoutGroup relevantLayout)
        {
            _layout = layout;
            _relevantLayout = relevantLayout;   
        }

        public void Padding(RectOffset padding) => _relevantLayout.padding = padding;
        
        public void Padding(PaddingSide side, int amount) => _relevantLayout.Padding(side, amount);
        public void PaddingAdd(PaddingSide side, int amount) => _relevantLayout.PaddingAdd(side, amount);
        public void ReverseArrangement(bool reverse = true) => _relevantLayout.reverseArrangement = reverse;

        public virtual void Add(params BaseComponent[] components)
        {
            _layout.Add(components);
        }
    }
}