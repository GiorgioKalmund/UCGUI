using UnityEngine;
using UnityEngine.UI;

namespace UCGUI {
    public class HorizontalSpacerBehaviour : ISpacerBehaviour
    {
        public void Apply(SpacerComponent spacer)
        {
            var layout = spacer.GetComponentInParent<HStackComponent>();
            if (layout == null)
            {
                UCGUILogger.LogError("HorizontalSpacerBehaviour: Cannot apply because no HStack is present in parent!\n");
                spacer.Enabled(false);
                return;
            }

            if (layout.ContentSizeFitter.enabled && layout.ContentSizeFitter.horizontalFit != ContentSizeFitter.FitMode.Unconstrained)
            {
                UCGUILogger.LogError("HorizontalSpacerBehaviour: Cannot apply because the parent HStack is dynamically sized.\nSpacers are currently only supported in statically sized layouts!\n");
                spacer.Enabled(false);
                return;
            }
            
            
            var width = layout.GetWidth();
            var spacing = layout.HorizontalLayout.spacing;
            var childCount = layout.transform.childCount;
            var childrenWidth = 0f;
            var spacerCount = 0;
            foreach (RectTransform child in layout.transform)
            {
                var go = child.gameObject;
                if (!go.GetComponent<SpacerComponent>())
                    childrenWidth += child.sizeDelta.x;
                else
                    spacerCount++;
            }

            float totalSpacing = (childCount - 1) * spacing;
            float totalPadding = layout.HorizontalLayout.padding.left + layout.HorizontalLayout.padding.right;
            var rest = width - childrenWidth - totalSpacing - totalPadding;
            var perSpacer = rest / spacerCount;
            
            //UCGUILogger.Log($"W:{width}, SP:{spacing}, CC:{childCount}, T:{totalSpacing}, C:{childrenWidth}, R:{rest}, S:{spacerCount}");
            
            if (rest < spacing)
            {
                UCGUILogger.LogWarning("HorizontalSpacerBehaviour: Cannot insert spacer. Not enough horizontal space! Disabled spacer.\n");
                spacer.Enabled(false);
            }
            spacer.Size(perSpacer, Defaults.Spacer.AlternateDirectionExtents);
            LayoutRebuilder.MarkLayoutForRebuild(layout.GetRect());
        }
    }
}