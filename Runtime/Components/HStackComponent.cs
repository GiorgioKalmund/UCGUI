using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class HStackComponent : HorizontalOrVerticalLayoutComponent
    {
        protected HStackComponent() {}
        
        public override void Awake()
        {
            base.Awake();
            AddHorizontalLayout();
        }

        protected override HorizontalOrVerticalLayoutGroup GetLayout()
        {
            return HorizontalLayout;
        }

        public override void Start()
        {
            base.Start();
            DisplayName = "HStack";
            HorizontalLayout.CalculateLayoutInputHorizontal();
        }
    }
}