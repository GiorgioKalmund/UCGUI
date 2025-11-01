using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class HStackComponent : HorizontalOrVerticalLayoutComponent
    {
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
        }
    }
}