using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class VStackComponent : HorizontalOrVerticalLayoutComponent
    {
        protected VStackComponent() {}
        
        protected override void Awake()
        {
            base.Awake();
            DisplayName = "VStack";
            
            AddVerticalLayout();
        }
        
        protected override void Start()
        {
            base.Start();
            VerticalLayout.CalculateLayoutInputVertical();
        }

        protected override HorizontalOrVerticalLayoutGroup GetLayout()
        {
            return VerticalLayout;
        }
    }
}