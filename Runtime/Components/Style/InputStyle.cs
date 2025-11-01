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
            input.GetTextComponent().Style(TextStyle.Primary);
            input.GetPlaceholderComponent().Style(TextStyle.Secondary);
            input.GetPlaceholderComponent().Alpha(0.55f);
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