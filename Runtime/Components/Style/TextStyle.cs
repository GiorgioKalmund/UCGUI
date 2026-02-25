using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    public class TextStyle : AbstractStyle<TextComponent, TextStyle>
    {
        // =============================================================== //
        //                        Static Text Styles                       //
        // =============================================================== //
        public static TextStyle Primary = new TextStyle(txt =>
        {
            txt.VAlignCenter().Color(Color.gray1);
        });

        public static TextStyle Secondary = Primary.Expand(txt =>
        {
            txt.Color(Color.gray);
        });
        
        public static TextStyle Tertiary = Primary.Expand(txt =>
        {
            txt.Color(Color.gray7);
        });
        
        public static TextStyle Caption = Primary.Expand(txt =>
        {
            txt.Color(Color.gray);
            txt.FontSize(txt.GetTextMesh().fontSize * 0.75f);
        });
                
        public static TextStyle ButtonText = Primary.Expand(txt =>
        {
            txt.AlignCenter().OverflowMode(TextOverflowModes.Ellipsis);
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