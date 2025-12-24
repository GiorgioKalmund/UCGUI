using UnityEngine.Events;

namespace UCGUI
{
    public class SliderStyle : AbstractStyle<SliderComponent, SliderStyle>
    {
        // =============================================================== //
        //                        Static Slider Styles                     //
        // =============================================================== //
      
        
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