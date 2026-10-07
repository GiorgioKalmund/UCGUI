using UCGUI.Styles;
using UCGUI.Support;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default Slider Component. <br></br><br></br>
    /// To create and customize it take a look at <see cref="UI.Slider"/> and the <see cref="SliderBuilder"/>.
    /// </summary>
    public class SliderComponent : BaseComponent,
        IStylable<SliderComponent, SliderStyle>
    {
        protected SliderComponent() {}
        
        public static readonly Vector2Int DefaultSize = new (160, 20); // This mirrors the default size when creating a uGUI Slider preset in the editor
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

        protected override void Awake()
        {
            base.Awake();

            slider = gameObject.GetOrAddComponent<Slider>();
            
            // Reference from uGUI Slider "Background"
            background = UI.Image().DisplayName("Background")
                .AnchorMin(0, 0.25f).AnchorMax(1, 0.75f)
                .Parent(this);

            _fillArea = UI.Blank(this, "Fill Area")
                .AnchorMin(0, 0.25f).AnchorMax(1, 0.75f);
            fill = UI.Image().DisplayName("Fill")
                .Parent(_fillArea)
                .ImageType(Image.Type.Sliced);

            _handleSlideArea = UI.Blank(slider, "Handle Slide Area")
                .Maximize()
                .Parent(slider);
            
            handle = UI.Image().DisplayName("Handle")
                    .Parent(_handleSlideArea);
            
            slider.targetGraphic = handle.GetImage();
            slider.handleRect = handle.GetRect();
            slider.fillRect = fill.GetRect();

            _fillArea.RectOffsets(new RectOffset(0, 0, 0, 0));
            fill.RectOffsets(new RectOffset(0, 0, 0, 0));
            _handleSlideArea.RectOffsets(new RectOffset(0, 0, 0, 0));
            handle.RectOffsets(new RectOffset(0, 0, 0, 0));
            handle.Width(20);
            background.RectOffsets(new RectOffset(0, 0, 0, 0));
            
            this.Size(DefaultSize);
            Style(SliderStyle.Default);
        }

        public SliderComponent FillAreaHorizontalPadding(int left, int right)
        {
            _fillArea.RectOffsets(new RectOffset(left, right, 0, 0));
            return this;
        }
        
        public SliderComponent HandleAreaHorizontalPadding(int left, int right)
        {
            _handleSlideArea.RectOffsets(new RectOffset(left, right, 0, 0));
            return this;
        }

        public SliderComponent HandleWidth(float width, bool adjustPaddingToKeepHandleInside = false)
        {
            handle.Width(width);
            if (adjustPaddingToKeepHandleInside)
                HandleAreaHorizontalPadding((int)width / 2, (int)width / 2);
            return this;
        }

        public SliderComponent WholeNumbers(bool wholeNumbers = true)
        {
            slider.wholeNumbers = wholeNumbers;
            return this;
        }

        public SliderComponent SetValue(float value)
        {
            Value = value;
            return this;
        }

        public SliderComponent SetRange(Range range)
        {
            if (!range.IsOrdered)
            {
                Debug.LogWarning("Slider range " + range + " was not ordered. Changed to: " + range.Flipped());
                range.Flip();
            }

            MinValue = range.minValue;
            MaxValue = range.maxValue;
            return this;
        }
        
        public virtual SliderComponent Interactable(bool interactable)
        {
            slider.interactable = interactable;
            return this;
        }

        public SliderComponent Style(SliderStyle style)
        {
            style.Apply(this);
            return this;
        }
    }
}