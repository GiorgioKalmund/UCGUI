using UnityEngine.UI;

namespace UCGUI
{
    public class VStackComponent : HorizontalOrVerticalLayoutComponent
    {
        protected VStackComponent() {}
        
        protected override void Awake()
        {
            base.Awake();
            AddVerticalLayout();
            RaycastTarget(false);
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