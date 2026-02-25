using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    public class SliderStyle : AbstractStyle<SliderComponent, SliderStyle>
    {
        // =============================================================== //
        //                        Static Slider Styles                     //
        // =============================================================== //
        public static SliderStyle Default = new SliderStyle(slider =>
        {
            slider.background.Color(Color.gray7);
            slider.fill.Color(Color.white);
            slider.handle.Color(Color.gray8);
        });
      
        
        // =============================================================== //
        //                        Implementation                           //
        // =============================================================== //
        public SliderStyle(UnityAction<SliderComponent> builder) : base(builder) { }
        protected override SliderStyle Create(UnityAction<SliderComponent> builder)
        {
            return new SliderStyle(builder);
        }
    }
}