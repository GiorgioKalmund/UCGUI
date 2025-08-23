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
            UCGUI.Defaults.TextDefaults.GlobalFont = Resources.Load<TMP_FontAsset>("Font/GoogleSansCode SDF");

            UCGUI.Window("Window", builder =>
            {
                
            }).Parent(canvas).Pos(300, 300);

            UCGUI.Button("Hello, World!", () =>
            {
                Debug.Log("Hello");
            }, label =>
            {
                label.FitToContents();
                label.Padding(new RectOffset(10, 10, 10, 10));
            }).Parent(canvas);

            var fish = UCGUI.Image(ImageService.GetSpriteFromAsset("player", "fish2"))
                .Parent(canvas);
            var fish2 = UCGUI.Image(ImageService.GetSpriteFromAsset("player", "fish"))
                .Offset(0, -200)
                .Parent(canvas);
            
            UCGUI.Text("Random Text", Color.coral).FitToContents().Pos(0, 100).Parent(canvas);

            UCGUI.Slider(new Range(2, -1), builder =>
            {

            }, newValue =>
            {
                Debug.Log("Slider Value Changed! " + newValue);
            }).Pos(0, 200).Parent(canvas);

            UCGUI.HStack(20, contents =>
            {
                contents.Add(fish,fish2);
            }).Parent(canvas);
            UCGUI.VStack(30, contents =>
            {
                contents.Add(fish.Copy(),fish2.Copy(), fish2.Copy());
            }).Parent(canvas);
        }
    }
}
