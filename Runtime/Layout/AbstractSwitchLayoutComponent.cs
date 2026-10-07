using System.Collections;
using UCGUI.Support;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public abstract class AbstractSwitchLayoutComponent<T>: HorizontalOrVerticalLayoutComponent, ICopyable<T> where T : AbstractSwitchLayoutComponent<T>
    {
        protected override void Awake()
        {
            base.Awake();
            
            if (Defaults.Layout.StandardLayoutDirection.Equals(ScrollViewDirection.Horizontal))
                MakeHorizontal();
            else if (Defaults.Layout.StandardLayoutDirection.Equals(ScrollViewDirection.Vertical))
                MakeVertical();
            else
                UCGUILogger.LogError($"Cannot create switch layout of type {GetType().Name}'. The default layout direction can only be ${ScrollViewDirection.Horizontal}' OR '{ScrollViewDirection.Vertical}'. It is: '{Defaults.Layout.StandardLayoutDirection}'");
        }

        public T MakeDirection(ScrollViewDirection dir)
        {
            if (IsDirection(dir))
            {
                UCGUILogger.LogWarning($"SwitchLayout: Attempting to switch to direction {dir.ToString()}, which the layout is already in. No action performed");
                return (T)this;
            }
            
            var tempChild = UI.N<BaseComponent>(this).DisplayName("TEMP_LAYOUT_CONVERSION_CHILD");
            if (dir.Equals(ScrollViewDirection.Horizontal))
            {
                tempChild.AddHorizontalLayout();
                tempChild.HorizontalLayout.CopyFrom(GetLayout());
            } else if (dir.Equals(ScrollViewDirection.Vertical))
            {
                tempChild.AddVerticalLayout();
                tempChild.VerticalLayout.CopyFrom(GetLayout());
            }
            else if (!dir.Equals(ScrollViewDirection.None))
            {
                // TODO: @Cleanup: Should maybe not even be an option --> Tailored enum only H, V, (or None?), no "Both"
            }
            
            // @Cleanup not optimal (see https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Object.DestroyImmediate.html),
            // but prevents breaking user expectations and removes headaches when trying to create and modify the layout immediately
            DestroyImmediate(GetLayout());
            
            if (dir.Equals(ScrollViewDirection.None)) // Early return, do not create any new layout
                return (T)this;
            
            UpdateDirection(dir, tempChild);
            return (T)this;
        }

        private IEnumerator UpdateDirectionAfterDestroy(ScrollViewDirection dir, BaseComponent tempChild)
        {
            yield return new WaitForEndOfFrame();
            UpdateDirection(dir, tempChild);
        }

        private void UpdateDirection(ScrollViewDirection dir, BaseComponent tempChild)
        {
            if (dir.Equals(ScrollViewDirection.Vertical))
            {
                HorizontalLayout = null;
                AddVerticalLayout(childControlWidth: true, childControlHeight: true);
            }
            if (dir.Equals(ScrollViewDirection.Horizontal))
            {
                VerticalLayout = null;
                AddHorizontalLayout(childControlWidth: true, childControlHeight: true);
            }
            GetLayout().CopyFrom(dir == ScrollViewDirection.Horizontal ? tempChild.HorizontalLayout : tempChild.VerticalLayout);
            Destroy(tempChild.gameObject);
        }

        public T MakeHorizontal() => MakeDirection(ScrollViewDirection.Horizontal);
        public T MakeVertical() => MakeDirection(ScrollViewDirection.Vertical);
        public bool IsDirection(ScrollViewDirection dir)
        {
            if (dir.Equals(ScrollViewDirection.Horizontal))
                return IsHorizontal();
            if (dir.Equals(ScrollViewDirection.Vertical))
                return IsVertical();
            return false;
        }

        public T SwitchDirection()
        {
            if (IsVertical())
                return MakeHorizontal();
            return MakeVertical();
        }

        public virtual T FitToContents(bool fit = true)
        {
            ContentSizeFitter.enabled = fit;
            GetLayout().childControlWidth = fit;
            GetLayout().childControlHeight = fit;
            return (T)this;
        }
        
        public new T Copy(bool fullyCopyRect = true)
        {
            var copyLabel = this.BaseCopy(this);
            return (T)copyLabel.CopyFrom(this, fullyCopyRect);
        }

        public T CopyFrom(T other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            HorizontalLayout.CopyFrom(other.HorizontalLayout);
            VerticalLayout.CopyFrom(other.VerticalLayout);

            if (other.ContentSizeFitter)
            {
                ContentSizeFitter.enabled = other.ContentSizeFitter.enabled;
                ContentSizeFitter.horizontalFit = other.ContentSizeFitter.horizontalFit;
                ContentSizeFitter.verticalFit = other.ContentSizeFitter.verticalFit;
            }
            return (T)this;
        }

        protected override HorizontalOrVerticalLayoutGroup GetLayout()
        {
            if (VerticalLayout)
                return VerticalLayout;
            return HorizontalLayout;
        }
    }
}