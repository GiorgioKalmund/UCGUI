using TMPro;
using UCGUI.Services;
using UnityEngine;

namespace UCGUI.Testing
{
    public class ComponentDemo : MonoBehaviour
    {
        private void Start()
        {
            var canvas = GUIService.GetCanvas();
            UI.Defaults.TextDefaults.GlobalFont = Resources.Load<TMP_FontAsset>("Font/GoogleSansCode SDF");

            UI.Window("Window", builder =>
            {
                
            }).Parent(canvas).Pos(300, 300);

            UI.Button("Hello, World!", () =>
            {
                Debug.Log("Hello");
            }, label =>
            {
                label.FitToContents();
                label.Padding(new RectOffset(10, 10, 10, 10));
            }).Parent(canvas);

            var fish = UI.Image(ImageService.GetSpriteFromAsset("player", "fish2"))
                .Parent(canvas);
            var fish2 = UI.Image(ImageService.GetSpriteFromAsset("player", "fish"))
                .Offset(0, -200)
                .Parent(canvas);
            
            UI.Text("Random Text", Color.coral).FitToContents().Pos(0, 100).Parent(canvas);

            UI.Slider(new Range(2, -1), builder =>
            {

            }, newValue =>
            {
                Debug.Log("Slider Value Changed! " + newValue);
            }).Pos(0, 200).Parent(canvas);

            UI.HStack(20, contents =>
            {
                contents.Add(fish,fish2);
            }).Parent(canvas);
            UI.VStack(30, contents =>
            {
                contents.Add(fish.Copy(),fish2.Copy(), fish2.Copy());
            }).Parent(canvas);
        }
    }
}
