using UnityEngine.Events;

namespace UCGUI
{
    public class FocusableComponent : BaseComponent, IFocusable
    {
        protected FocusableComponent() {}
        public virtual string FocusGroup { get; set; }
        public UnityEvent OnFocusEvent { get; set; }
        
        public UnityEvent OnUnfocusEvent { get; set; }

        public void HandleFocus() { }

        public void HandleUnfocus() { }
    }
}
