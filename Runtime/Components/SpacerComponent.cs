using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public interface ISpacerBehaviour
    {
        void Apply(SpacerComponent spacer);
    }

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
            //Debug.Log($"W:{width}, SP:{spacing}, CC:{childCount}, T:{totalSpacing}, C:{childrenWidth}, R:{rest}, S:{spacerCount}");
            if (rest < spacing)
            {
                UCGUILogger.LogWarning("HorizontalSpacerBehaviour: Cannot insert spacer. Not enough horizontal space! Disabled spacer.\n");
                spacer.Enabled(false);
            }
            spacer.Size(perSpacer, Defaults.Spacer.AlternateDirectionExtents);
            LayoutRebuilder.MarkLayoutForRebuild(layout.GetRect());
        }
    }

    public class VerticalSpacerBehaviour : ISpacerBehaviour
    {
        public void Apply(SpacerComponent spacer)
        {
            var layout = spacer.GetComponentInParent<VStackComponent>();
            if (layout == null)
            {
                UCGUILogger.LogError("VerticalSpacerBehaviour: Cannot apply because no VStack is present in parent!");
                spacer.Enabled(false);
                return;
            }

            if (layout.ContentSizeFitter.enabled && layout.ContentSizeFitter.verticalFit != ContentSizeFitter.FitMode.Unconstrained)
            {
                UCGUILogger.LogError("VerticalSpacerBehaviour: Cannot apply because the parent VStack is dynamically sized.\nSpacers are currently only supported in statically sized layouts!");
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
            //Debug.Log($"W:{width}, SP:{spacing}, CC:{childCount}, T:{totalSpacing}, C:{childrenWidth}, R:{rest}, S:{spacerCount}");
            if (rest < spacing)
            {
                UCGUILogger.LogWarning("VerticalSpacerBehaviour: Cannot insert spacer. Not enough vertical space!\nDisabled spacer.");
                spacer.Enabled(false);
            }
            spacer.Size(Defaults.Spacer.AlternateDirectionExtents, perSpacer);
            LayoutRebuilder.MarkLayoutForRebuild(layout.GetRect());
        }
        
    }

    public class SpacerComponent : BaseComponent, IEnabled, IRenderable
    {
        private ISpacerBehaviour _behaviour;

        public virtual void Start()
        {
            this.SafeDisplayName("Spacer");
            
            if (_behaviour == null)
                AutoSetBehaviour();
            
            if (_behaviour == null)
            {
                UCGUILogger.LogError("Spacer does not have a behaviour attached! Please specify one!");
                return;
            }

            Render();
        }

        public SpacerComponent SetBehaviour(ISpacerBehaviour behaviour)
        {
            if (behaviour == null)
            {
                UCGUILogger.LogError($"ISpacerBehaviour cannot be null!");
                return this;
            }

            if (_behaviour != null)
            {
                UCGUILogger.LogError($"Spacer does already a behaviour attached! Cannot additionally add {behaviour.GetType()}\nIf you really want this Spacer to have a different behaviour please use 'ReplaceBehaviour' instead.");
                return this;
            }

            _behaviour = behaviour;
            return this;
        }

        public void ReplaceBehaviour(ISpacerBehaviour behaviour)
        {
            if (behaviour == null)
            {
                UCGUILogger.LogError($"ISpacerBehaviour cannot be null!");
                return;
            }
            
            _behaviour = behaviour;
            _behaviour.Apply(this);
        }

        public void AutoSetBehaviour(bool overrideExisting = false)
        {
            if (!overrideExisting && _behaviour != null)
            {
                UCGUILogger.LogError("An existing behaviour already attached. Cannot auto set. Set 'overrideExisting' to true to set a new behaviour.");
                return;
            }
            
            if (gameObject.GetComponentInParent<HStackComponent>())
                SetBehaviour(new HorizontalSpacerBehaviour());
            else if (gameObject.GetComponentInParent<VStackComponent>())
                SetBehaviour(new VerticalSpacerBehaviour());
            else
            {
                UCGUILogger.LogError("Spacer: Cannot auto set behaviour based on parent. Is the spacer part of a valid hierarchy?");
            }
        }

        public void Enabled(bool on)
        {
            gameObject.SetActive(on);
        }

        public void Render()
        {
            // We wait here to allow all sub-hierarchies and layouts to also build beforehand, otherwise the 
            // calculated spacing might be incorrect
            StartCoroutine(RenderRoutine());
        }

        private IEnumerator RenderRoutine()
        {
            yield return new WaitForEndOfFrame();
            _behaviour.Apply(this);
        }
    }
}