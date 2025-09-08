using UnityEngine.Events;

namespace UCGUI
{
    public class ButtonStyle : AbstractStyle<ButtonComponent, ButtonStyle>
    {
        public ButtonStyle(UnityAction<ButtonComponent> builder) : base(builder)
        {
            
        }

        protected override ButtonStyle Create(UnityAction<ButtonComponent> builder)
        {
            return new ButtonStyle(builder);
        }
    }
}