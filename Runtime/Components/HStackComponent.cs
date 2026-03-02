using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class HStackComponent : HorizontalOrVerticalLayoutComponent
    {
        protected HStackComponent() {}
        
        protected override void Awake()
        {
            base.Awake();
            DisplayName = "HStack";
            
            AddHorizontalLayout();
        }

        protected override HorizontalOrVerticalLayoutGroup GetLayout()
        {
            return HorizontalLayout;
        }

        protected override void Start()
        {
            base.Start();
            HorizontalLayout.CalculateLayoutInputHorizontal();
        }
    }
}