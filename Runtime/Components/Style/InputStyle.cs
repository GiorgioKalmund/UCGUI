using UnityEngine.Events;

namespace UCGUI
{
    public class InputStyle : AbstractStyle<InputComponent, InputStyle>
    {
        // =============================================================== //
        //                        Static Input Styles                      //
        // =============================================================== //
        public static readonly InputStyle Default = new InputStyle(input =>
        {
            input.TextElement.Style(TextStyle.Primary);
            input.PlaceholderElement.Style(TextStyle.Secondary);
            input.PlaceholderElement.Alpha(0.55f);
        });
        
        // =============================================================== //
        //                        Implementation                           //
        // =============================================================== //
        public InputStyle(UnityAction<InputComponent> builder) : base(builder) { }
        protected override InputStyle Create(UnityAction<InputComponent> builder)
        {
            return new InputStyle(builder);
        }
    }
}