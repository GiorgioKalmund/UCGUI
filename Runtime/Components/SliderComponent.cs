using UCGUI.Services;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default Slider Component. <br></br><br></br>
    /// To create and customize it take a look at <see cref="UI.Slider"/> and the <see cref="SliderBuilder"/>.
    /// </summary>
    public partial class SliderComponent : BaseComponent
    {
        public static readonly Vector2 DefaultSize = new (300, 20);
        
        public Slider slider;
        
        // -- Subcomponents -- //
        public ImageComponent background;
        private ImageComponent _fillArea;
        public ImageComponent foreground;
        private ImageComponent _handleSlideArea;
        public ImageComponent handle;

        public Slider.SliderEvent OnValueChanged => slider.onValueChanged;
        
        public float Value
        {
            get => slider.value;
            set => slider.value = value;
        }

        public float MaxValue
        {
            get => slider.maxValue;
            set => slider.maxValue = value;
        }
        public float MinValue 
        {
            get => slider.minValue;
            set => slider.minValue= value;
        }

        public override void Awake()
        {
            base.Awake();

            slider = gameObject.GetOrAddComponent<Slider>();

            background = UI.Image(ImageService.White).Color(Color.gray7).Parent(slider).DisplayName("Background").Stretch();
            _fillArea = UI.Image(null).Alpha(0).Stretch().Parent(slider).DisplayName("Fill Area");
            foreground = UI.Image(ImageService.White).Parent(_fillArea).Filled(Image.FillMethod.Horizontal, Value).DisplayName("Foreground");
            _handleSlideArea = UI.Image(null).Alpha(0).Stretch().Parent(slider).DisplayName("Handle Slide Area");
            handle = UI.Image(ImageService.White).Color(Color.gray3).Parent(_handleSlideArea).DisplayName("Handle");

            slider.targetGraphic = handle.GetImage();
            slider.handleRect = handle.GetRect();
            slider.fillRect = foreground.GetRect();

            foreground.RectOffsets(new RectOffset(0, 0, 0, 0));
            handle.RectOffsets(new RectOffset(0, 0, 0, 0));
            handle.Pivot(PivotPosition.MiddleLeft, true).Pos(0, 0);

            this.Size(DefaultSize);
        }

        private void Start()
        {
            DisplayName = "Slider";
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);
            background.Size(this.GetSize());
            _handleSlideArea.Size(x, y);
            _fillArea.Size(this.GetSize());
            return this;
        }

        public void IntegerSteps(bool steps = true)
        {
            slider.wholeNumbers = steps;
        }

        /// <summary>
        /// Builder for UCGUI <see cref="SliderComponent"/>.<br/>
        /// <b>Builder Functions:</b>
        /// <list type="bullet">
        /// <item><description><see cref="Value"/> - Sets the initial value of the slider.</description></item>
        /// <item><description><see cref="IntegerSteps(bool)"/> - Forces the steps to be whole numbers.</description></item>
        /// <item><description><see cref="SliderBuilder.Foreground(UnityEngine.Sprite,UnityEngine.Color?)"/> - Sets the sprite of the foreground / fill area. Optionally changes the color as well.</description></item>
        /// <item><description><see cref="SliderBuilder.Background(UnityEngine.Sprite,UnityEngine.Color?)"/> - Sets the sprite of the background. Optionally changes the color as well.</description></item>
        /// <item><description><see cref="SliderBuilder.Handle(UnityEngine.Sprite,UnityEngine.Color?)"/> - Sets the sprite of the handle. Optionally changes the color as well.</description></item>
        /// <item><description><see cref="SliderBuilder.HandleWidth(float)"/> - Sets the width of the slider's handle.</description></item>
        /// <item><description><see cref="HandleExtraHeight"/> - Adds a given delta to the height. <i>It is this way as per default the Unity slider behind the scenes stretches the height to be the height of the slider itself.</i></description></item>
        /// </list>
        /// </summary>
        public partial class SliderBuilder
        {
            private SliderComponent _slider;

            public SliderBuilder(SliderComponent slider)
            {
                _slider = slider;
            }
            
            public void Foreground(Sprite sprite, Color? color = null) { _slider.foreground.Sprite(sprite); if (color.HasValue) Foreground(color.Value);}
            public void Foreground(Color color) { _slider.foreground.Color(color); }
            public void Background(Sprite sprite, Color? color = null) { _slider.background.Sprite(sprite); if (color.HasValue) Background(color.Value);}
            public void Background(Color color) { _slider.background.Color(color); }
            public void Handle(Sprite sprite, Color? color = null) { _slider.handle.Sprite(sprite); if (color.HasValue) Handle(color.Value);}
            public void Handle(Color color) { _slider.handle.Color(color); }
            public void HandleWidth(float x) { _slider.handle.Width(x); }
            public void HandleExtraHeight(float delta)
            {
                float half = delta * 0.5f;
                RectTransform rt = _slider.handle.GetRect();

                if (!Mathf.Approximately(rt.anchorMin.y, rt.anchorMax.y))
                {
                    rt.offsetMin = new Vector2(rt.offsetMin.x, rt.offsetMin.y - half);
                    rt.offsetMax = new Vector2(rt.offsetMax.x, rt.offsetMax.y + half);
                }
            }
            public void Value(float val) { _slider.Value = val; }
            public void IntegerSteps(bool steps = true) { _slider.IntegerSteps(steps); }
        }
    }
}