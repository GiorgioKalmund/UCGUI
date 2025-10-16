using UnityEngine.Events;

namespace UCGUI
{
    public class InputStyle : AbstractStyle<InputComponent, InputStyle>
    {
        public InputStyle(UnityAction<InputComponent> builder) : base(builder)
        {
            
        }

        protected override InputStyle Create(UnityAction<InputComponent> builder)
        {
            return new InputStyle(builder);
        }
    }
}