using UCGUI.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UCGUI.Demo
{
    public class DemoMenu : SimpleScreen
    {
        public override void Setup()
        {
            DisplayName = "DemoMenu";

            ColorUtility.TryParseHtmlString("#2e222f", out Color backdropColor);
            
            var backdrop = ComponentExtension.N<ImageComponent>("Backdrop", transform)
                    .FullScreen(canvas)
                    .Color(backdropColor)
                ;

            var title = ComponentExtension.N<TextComponent>("Title", transform)
                    .Text("Component: UGUI Demo").FitToContents().FontSize(70).Bold()
                    .Pivot(PivotPosition.UpperCenter, true).Offset(0, -50)
                ;

            var buttonParent = ComponentExtension.N<ImageComponent>("Menu Buttons", transform)
                    .Size(1500, 800)
                    .Offset(0, -45)
                    .Sprite(ImageService.GetSpriteDirectly("Slice/Slot")).ImageType(Image.Type.Sliced).PixelsPerUnitMultiplier(0.33f)
                    .Color(Color.gray6)
                    .AddVerticalLayout(childAlignment:TextAnchor.UpperLeft, spacing:9)
                    .Padding(25, ScrollViewDirection.Vertical)
                ;
            
            var sceneButton1 = ComponentExtension.N<ButtonComponent>("Scene 1").Parent(buttonParent)
                    .Size(500, 100)
                    .Create("The Basics", foreground:ImageService.GetSpriteFromAsset("player", "head"))
                    .SpriteSwap(ImageService.GetSpriteDirectly("Slice/Slot Selected"))
                    .Sprite(ImageService.GetSpriteDirectly("Slice/Slot")).ImageType(Image.Type.Sliced).PixelsPerUnitMultiplier(0.33f)
                    .AddHorizontalLayout(childAlignment:TextAnchor.MiddleLeft, spacing:10).Padding(PaddingSide.Leading, 25, ScrollViewDirection.Horizontal)
                    .Pivot(PivotPosition.MiddleLeft, true)
                    .Cast<ButtonComponent>()
                ;
            sceneButton1.GetTextComponent().FitToContents().Bold().Color(Color.white);
            sceneButton1.GetForeground().Size(60, 60);

            var sceneButton2 = sceneButton1.Copy().Function(() => SceneManager.LoadScene("Demo/")).Text("Images & Animations").Foreground(ImageService.GetSpriteFromAsset("player", "booster paddels single")).GetForeground().NativeSize(2, 2);
            var sceneButton3 = sceneButton1.Copy().Function(() => SceneManager.LoadScene("Demo/")).Text("Popups").Foreground(ImageService.GetSpriteFromAsset("player", "booster paddels")).GetForeground().NativeSize(1.5f, 1.5f);
            var sceneButton4 = sceneButton1.Copy().Function(() => SceneManager.LoadScene("Demo/")).Text("Focus").Foreground(ImageService.GetSpriteFromAsset("player", "torch")).GetForeground().NativeSize(2, 2);
            var sceneButton5 = sceneButton1.Copy().Function(() => SceneManager.LoadScene("Demo/")).Text("Scroll Views").Foreground(ImageService.GetSpriteFromAsset("player", "rock")).GetForeground().NativeSize(2, 2);
            var sceneButton6 = sceneButton1.Copy().Function(() => SceneManager.LoadScene("Demo/")).Text("Input").Foreground(ImageService.GetSpriteFromAsset("player", "rock")).GetForeground().NativeSize(2, 2);
            var sceneButton7 = sceneButton1.Copy().Function(() => SceneManager.LoadScene("Demo/")).Text("Windows").Foreground(ImageService.GetSpriteFromAsset("player", "rock")).GetForeground().NativeSize(2, 2);
        }

        public override Canvas GetCanvas()
        {
            return GUIService.GetCanvas();
        }
    }
}