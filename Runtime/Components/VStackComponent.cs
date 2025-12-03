using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class VStackComponent : HorizontalOrVerticalLayoutComponent
    {
        protected VStackComponent() {}
        
        public override void Awake()
        {
            base.Awake();
            AddVerticalLayout();
        }

        protected override HorizontalOrVerticalLayoutGroup GetLayout()
        {
            return VerticalLayout;
        }

        public override void Start()
        {
            base.Start();
            DisplayName = "VStack";
            VerticalLayout.CalculateLayoutInputVertical();
        }
    }
}