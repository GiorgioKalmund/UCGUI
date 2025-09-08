using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UCGUI.Game
{
    /// <summary>
    /// UCGUI's default <see cref="WheelMenu"/>'s Button.
    /// Automatically focuses the button when the pointer enters, and defocuses when the pointer exits.
    /// Also reorients the text and foreground image to align expected.
    /// </summary>
    public partial class WheelMenuButton : ButtonComponent
    {
        public override void Start()
        {
            base.Start();

            ForegroundImage.Rotation(0);
            ButtonText.Rotation(0);
        }

        public override void HandlePointerEnter(PointerEventData eventData)
        {
            base.HandlePointerEnter(eventData);
            this.Focus();
        }

        public override void HandlePointerExit(PointerEventData eventData)
        {
            base.HandlePointerEnter(eventData);
            this.UnFocus();
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
    }
}