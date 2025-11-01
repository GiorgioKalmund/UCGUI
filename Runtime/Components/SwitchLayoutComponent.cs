using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class SwitchLayoutComponent<T>: HorizontalOrVerticalLayoutComponent, ICopyable<T> where T : SwitchLayoutComponent<T>
    {
        public override void Awake()
        {
            base.Awake();
            MakeHorizontal();
        }

        public override void Start()
        {
            base.Start();
            DisplayName = "Switch Layout";
        }

        public virtual T MakeDirection(ScrollViewDirection dir)
        {
            if (IsDirection(dir))
            {
                UCGUILogger.LogWarning($"SwitchLayout: Attempting to switch to direction {dir.ToString()}, which the layout is already in. No action performed");
                return (T)this;
            }
            
            var tempChild = UI.N<BaseComponent>(this, "TEMP_LAYOUT_CONVERSION_CHILD");
            if (dir.Equals(ScrollViewDirection.Horizontal))
            {
                tempChild.AddHorizontalLayout();
                tempChild.HorizontalLayout.CopyFrom(GetLayout());
            } else if (dir.Equals(ScrollViewDirection.Vertical))
            {
                tempChild.AddVerticalLayout();
                tempChild.VerticalLayout.CopyFrom(GetLayout());
            }
            
            Destroy(GetLayout());
            
            // Resort to this because previous layout is only destroyed at end of frame. 
            // This is done to not use DestroyImmediate(GetLayout()), which is strongly advised against using.
            if (GetLayout())
                StartCoroutine(UpdateDirectionAfterDestroy(dir, tempChild));
            else
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
        public bool IsVertical() => VerticalLayout != null;
        public bool IsHorizontal() => HorizontalLayout != null;
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
                FitToContents();
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