using UnityEngine.Events;

namespace UCGUI
{
    public class TextStyle : AbstractStyle<TextComponent, TextStyle>
    {
        public TextStyle(UnityAction<TextComponent> builder) : base(builder)
        {
            
        }

        protected override TextStyle Create(UnityAction<TextComponent> builder)
        {
            return new TextStyle(builder);
        }
    }
}