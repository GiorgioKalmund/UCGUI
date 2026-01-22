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
            slider.background.Color(Color.gray9);
            slider.fill.Color(Color.white);
            slider.handle.Color(Color.white);
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