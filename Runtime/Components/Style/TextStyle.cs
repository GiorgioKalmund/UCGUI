using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    public class TextStyle : AbstractStyle<TextComponent, TextStyle>
    {
        // =============================================================== //
        //                        Static Text Styles                       //
        // =============================================================== //
        public static readonly TextStyle Primary = new TextStyle(txt =>
        {
            txt.VAlignCenter().Color(Color.gray1);
        });

        public static readonly TextStyle Secondary = Primary.Expand(txt =>
        {
            txt.Color(Color.gray);
        });
                
        public static readonly TextStyle ButtonText = Primary.Expand(txt =>
        {
            txt.AlignCenter();
        }); 
        
        
        // =============================================================== //
        //                        Implementation                           //
        // =============================================================== //
        public TextStyle(UnityAction<TextComponent> builder) : base(builder) { }
        protected override TextStyle Create(UnityAction<TextComponent> builder)
        {
            return new TextStyle(builder);
        }
    }
}