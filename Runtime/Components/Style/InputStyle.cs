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
            input.text.Style(TextStyle.Primary);
            input.placeholder.Style(TextStyle.Secondary);
            input.placeholder.Alpha(0.55f);
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