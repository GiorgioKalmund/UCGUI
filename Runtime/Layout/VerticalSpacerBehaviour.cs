using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class VerticalSpacerBehaviour : ISpacerBehaviour
    {
        public void Apply(SpacerComponent spacer)
        {
            var layout = spacer.GetComponentInParent<HorizontalOrVerticalLayoutComponent>();
            if (layout == null)
            {
                UCGUILogger.LogError("VerticalSpacerBehaviour: Cannot apply because no vertical layout is present in parent!");
                spacer.Enabled(false);
                return;
            }

            if (!layout.IsVertical())
            {
                UCGUILogger.LogError("VerticalSpacerBehaviour: Cannot apply because the parent layout is not vertical. Please make sure the parent contains a VerticalLayoutGroup.");
                spacer.Enabled(false);
                return;
            }
            
            if (layout.ContentSizeFitter.enabled && layout.ContentSizeFitter.verticalFit != ContentSizeFitter.FitMode.Unconstrained)
            {
                UCGUILogger.LogError("VerticalSpacerBehaviour: Cannot apply because the parent vertical layout is dynamically sized.\nSpacers are currently only supported in statically sized layouts! Did you forget to add a fixed HEIGHT to the container?");
                spacer.Enabled(false);
                return;
            }
            
            var height = layout.GetHeight();
            var spacing = layout.VerticalLayout.spacing;
            var childCount = layout.transform.childCount;
            var childrenHeight = 0f;
            var spacerCount = 0;
            foreach (RectTransform child in layout.transform)
            {
                var go = child.gameObject;
                if (!go.GetComponent<SpacerComponent>())
                    childrenHeight += child.sizeDelta.y;
                else
                    spacerCount++;
            }

            float totalSpacing = (childCount - 1) * spacing;
            float totalPadding = layout.VerticalLayout.padding.top + layout.VerticalLayout.padding.bottom;
            var rest = height - childrenHeight - totalSpacing - totalPadding;
            var perSpacer = rest / spacerCount;
            
            //UCGUILogger.Log($"W:{height}, SP:{spacing}, CC:{childCount}, T:{totalSpacing}, C:{childrenHeight}, R:{rest}, S:{spacerCount}");
            
            if (rest < spacing)
            {
                UCGUILogger.LogWarning("VerticalSpacerBehaviour: Cannot insert spacer. Not enough vertical space!\nDisabled spacer.");
                spacer.Enabled(false);
            }

            spacer.Size(Defaults.Spacer.AlternateDirectionExtents, perSpacer);
            spacer.PreferredSize(Defaults.Spacer.AlternateDirectionExtents, perSpacer);
            LayoutRebuilder.MarkLayoutForRebuild(layout.GetRect());
        }
        
    }
}    
