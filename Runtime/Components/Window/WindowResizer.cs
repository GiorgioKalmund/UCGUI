using UCGUI.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UCGUI
{
    public class WindowResizer : ImageComponent, IDragHandler 
    {
        private ResizableWindowComponent _resizableWindow;

        public override void Start()
        {
            base.Start();
            DisplayName = "WindowResizer";
        }

        public WindowResizer Build(ResizableWindowComponent resizableWindow)
        {
            _resizableWindow = resizableWindow;
            return this;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            var size = _resizableWindow.GetRect().sizeDelta;
            float newWidth = size.x + eventData.delta.x;
            float newHeight = size.y - eventData.delta.y;
            
            Vector2 newSize = new Vector2(Mathf.Min(GUIService.GetCanvasWidth(), Mathf.Max(newWidth, _resizableWindow.GetMinimumSize().x)), Mathf.Min(GUIService.GetCanvasHeight(), Mathf.Max(newHeight, _resizableWindow.GetMinimumSize().y)));
            _resizableWindow.Size(newSize).Render();
        }
    }
}