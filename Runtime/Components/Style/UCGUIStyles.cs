using UCGUI.Services;
using UnityEngine;

namespace UCGUI
{
    public class UCGUIStyles
    {
        public static class Button
        {
            public static readonly ButtonStyle Default = new ButtonStyle(btn =>
            {
                btn.TextBuilder().Style(Text.ButtonText);
            });

            public static readonly ButtonStyle DefaultFit = Default.Expand(btn =>
            {
                btn.FitToContents(20, 20);
            });
        }

        public static class Text
        {
            public static readonly TextStyle ButtonText = new TextStyle(txt =>
            {
                txt.VAlignCenter().AlignCenter().Color(Color.gray1);
            });
        }
    }
}