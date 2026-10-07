using UnityEngine;
using UnityEngine.Events;

namespace UCGUI.Styles
{
   public class ScrollViewStyle : AbstractStyle<ScrollViewComponent, ScrollViewStyle>
    {
        // =============================================================== //
        //                       Static Button Styles                      //
        // =============================================================== //
        public static ScrollViewStyle Plain => new ScrollViewStyle(scrollView =>
        {
            scrollView.Color(Color.white, 0.1f);
        });
        
        public static ImageStyle PlainBar => new ImageStyle(bar =>
        {
            bar.Color(Color.gray, 0.8f);
        });
        
        public static ImageStyle PlainHandle => new ImageStyle(handle=>
        {
            
        });
        
        // =============================================================== //
        //                        Implementation                           //
        // =============================================================== //
        public ScrollViewStyle(UnityAction<ScrollViewComponent> builder) : base(builder) { }
        protected override ScrollViewStyle Create(UnityAction<ScrollViewComponent> builder)
        {
            return new ScrollViewStyle(builder);
        }
    }
}