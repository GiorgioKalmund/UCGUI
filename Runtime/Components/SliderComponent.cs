using System.Collections;
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
    public partial class SliderComponent : BaseComponent,
        IStylable<SliderComponent, SliderStyle>
    {
        protected SliderComponent() {}
        
        public static readonly Vector2Int DefaultSize = new (300, 20);
        
        public Slider slider;
        
        // -- Subcomponents -- //
        public ImageComponent background;
        private BaseComponent _fillArea;
        public ImageComponent fill;
        private BaseComponent _handleSlideArea;
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
            
            var missingSprite = ImageService.MissingSprite;

            this.Size(DefaultSize);
            

            background = UI.Image(missingSprite).Parent(slider).DisplayName("Background")
                    .AnchorMin(0, 0.25f).AnchorMax(1, 0.75f)
                    .RectOffsets(new RectOffset(0, 0, 0, 0))
                ;
            _fillArea = UI.N<BaseComponent>()
                .AnchorMin(0, 0.25f).AnchorMax(1, 0.75f)
                .RectOffsets(new RectOffset(5, 15, 0, 0))
                .Parent(slider).DisplayName("Fill Area");
            fill = UI.Image(missingSprite).Parent(_fillArea)
                .StretchVertically(VerticalStretchAnchor.Center)
                .Filled(Image.FillMethod.Horizontal, Value).DisplayName("Fill")
                .RectOffsets(new RectOffset(-5, -5, 0, 0))
                ;
            _handleSlideArea = UI.N<BaseComponent>().AnchorMin(0, 0).AnchorMax(1, 1)
                .RectOffsets(new RectOffset(DefaultSize.y / 2, DefaultSize.y / 2, 0, 0))
                .Parent(slider).DisplayName("Handle Slide Area");
            handle = UI.Image(missingSprite)
                .Color(Color.gray3).Parent(_handleSlideArea).DisplayName("Handle")
                .RectOffsets(new RectOffset(0, DefaultSize.y, 0, 0))
                .Pos(0, 0)
                ;
            
            slider.targetGraphic = handle.GetImage();
            slider.handleRect = handle.GetRect();
            slider.fillRect = fill.GetRect();

            Style(SliderStyle.Default);
        }

        public void Lock()
        {
            slider.interactable = false;
        }
        public void Unlock()
        {
            slider.interactable = true;
        }

        private void Start()
        {
            DisplayName = "Slider";
        }

        public override BaseComponent HandleSizeChanged(float x, float y)
        {
            base.HandleSizeChanged(x, y);
            handle?.RectOffsets(new RectOffset(0, -(int)y, 0, 0))
                .Pos(0, 0);
            return this;
        }

        public void IntegerSteps(bool steps = true)
        {
            slider.wholeNumbers = steps;
        }

        public void SetRange(Range range)
        {
            if (!range.IsOrdered)
            {
                Debug.LogWarning("Slider range " + range + " was not ordered. Changed to: " + range.Flipped());
                range.Flip();
            }

            MinValue = range.minValue;
            MaxValue = range.maxValue;
        }

        /// <summary>
        /// Builder for UCGUI <see cref="SliderComponent"/>.<br/>
        /// <b>Builder Functions:</b>
        /// <list type="bullet">
        /// <item><description><see cref="Value"/> - Sets the initial value of the slider.</description></item>
        /// <item><description><see cref="IntegerSteps(bool)"/> - Forces the steps to be whole numbers.</description></item>
        /// <item><description><see cref="SliderBuilder.Fill(UnityEngine.Sprite,UnityEngine.Color?)"/> - Sets the sprite of the foreground / fill area. Optionally changes the color as well.</description></item>
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
            
            public void Fill(Sprite sprite, Color? color = null) { _slider.fill.Sprite(sprite); if (color.HasValue) Fill(color.Value);}
            public void Fill(Color color) { _slider.fill.Color(color); }
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

        public SliderComponent Style(SliderStyle style)
        {
            style.Apply(this);
            return this;
        }
    }
}