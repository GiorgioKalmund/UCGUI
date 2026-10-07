using UCGUI.Support;
using UnityEngine;

namespace UCGUI
{
    using UnityEngine.UI;
    public class LayoutBuilder
    {
        private HorizontalOrVerticalLayoutComponent _layout;
        private HorizontalOrVerticalLayoutGroup _relevantLayout;

        public LayoutBuilder(HorizontalOrVerticalLayoutComponent layout, HorizontalOrVerticalLayoutGroup relevantLayout)
        {
            _layout = layout;
            _relevantLayout = relevantLayout;   
        }

        public void Padding(RectOffset padding) => _relevantLayout.padding = padding;
        
        public void Padding(int amount) => _relevantLayout.Padding(PaddingSide.All, amount);
        public void Padding(PaddingSide side, int amount) => _relevantLayout.Padding(side, amount);
        public void PaddingAdd(PaddingSide side, int amount) => _relevantLayout.PaddingAdd(side, amount);
        public void ReverseArrangement(bool reverse = true) => _relevantLayout.reverseArrangement = reverse;

        public HorizontalOrVerticalLayoutComponent GetLayout() => _layout;

        public void Add(params BaseComponent[] components)
        {
            foreach (var baseComponent in components)
            {
                // If this elemnt is already part of this layout, we simply re-organize it to be at the top,
                // resulting in the same behaviour
                if (_layout.transform.Equals(baseComponent.transform.parent))
                    baseComponent.BringToFront();
                else
                    baseComponent.Parent(_layout);
            }
        }

        public void Spacer()
        {
            UI.Spacer();
        }
    }
}