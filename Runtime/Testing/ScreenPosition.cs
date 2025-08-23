using UCGUI.Services;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UCGUI.Testing
{
    public class ScreenPosition : MonoBehaviour 
    {
        private void OnDrawGizmos()
        {
            var screenPos = gameObject.transform.position.ToCanvasPos(Camera.main, GUIService.GetCanvas()); ;
            var worldPos = screenPos.ToWorldPos(Camera.main, GUIService.GetCanvas(), gameObject.transform.position.z);
            Handles.Label(gameObject.transform.position, $"Pos: {(int)screenPos.x} x {(int)screenPos.y}\nWorldPos?: {worldPos.x}x{worldPos.y}x{worldPos.z}");
        }
    }
}
#endif