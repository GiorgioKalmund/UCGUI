using UnityEngine.Events;

namespace UCGUI
{
    public class ButtonStyle : AbstractStyle<ButtonComponent, ButtonStyle>
    {
        // =============================================================== //
        //                       Static Button Styles                      //
        // =============================================================== //
        public static ButtonStyle Plain => new ButtonStyle(button=>
        {
            button.Color(UnityEngine.Color.white);
            button.FitToContents(20, 20);
        });
        
        // =============================================================== //
        //                        Implementation                           //
        // =============================================================== //
        public ButtonStyle(UnityAction<ButtonComponent> builder) : base(builder) { }
        protected override ButtonStyle Create(UnityAction<ButtonComponent> builder)
        {
            return new ButtonStyle(builder);
        }
    }
}