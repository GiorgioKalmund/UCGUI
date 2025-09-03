using UnityEditor;
using UnityEngine;

namespace UCGUI.Demo
{
    public class HotbarSlot : ButtonComponent, IFocusable, ICopyable<HotbarSlot>
    {
        // -- Focus -- //
        public override void HandleFocus()
        {
            base.HandleFocus();
            Sprite("player", "Inventory Slot Selected");
        }

        public override void HandleUnfocus()
        {
            base.HandleUnfocus();
            Sprite("player", "Inventory Slot");
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = UnityEngine.Color.red;
            style.fontSize = 14; 
            Handles.Label(transform.position, GetFocusGroup() + "", style);
        }
        #endif
        
        // -- Copy -- //
        public new HotbarSlot Copy(bool fullyCopyRect = true)
        {
            HotbarSlot copySlot = this.BaseCopy(this);
            return copySlot.CopyFrom(this, fullyCopyRect);
        }

        public HotbarSlot CopyFrom(HotbarSlot other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            return this;
        }

        private void OnDestroy()
        {
            this.UnFocus();
        }
    }
}