using UnityEngine.Events;

namespace UCGUI
{
    public class ImageStyle : AbstractStyle<ImageComponent, ImageStyle>
    {
        // =============================================================== //
        //                       Static Image Styles                       //
        // =============================================================== //
        

        // =============================================================== //
        //                        Implementation                           //
        // =============================================================== //
        public ImageStyle(UnityAction<ImageComponent> builder) : base(builder) { }
        protected override ImageStyle Create(UnityAction<ImageComponent> builder)
        {
            return new ImageStyle(builder);
        }
    }
}