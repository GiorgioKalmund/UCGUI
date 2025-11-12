using UnityEngine.Events;

namespace UCGUI
{
    public class LabelStyle : AbstractStyle<LabelComponent, LabelStyle>
    {
        // =============================================================== //
        //                        Static Label Styles                      //
        // =============================================================== //
        public static LabelStyle IconOnly => new LabelStyle(label =>
        {
            if (label.HasImage()) label.image.gameObject.SetActive(true);
            if (label.HasText()) label.text.gameObject.SetActive(false);
        });
        
        public static LabelStyle TextOnly => new LabelStyle(label =>
        {
            if (label.HasImage()) label.image.gameObject.SetActive(false);
            if (label.HasText()) label.text.gameObject.SetActive(true);
        });
        
        public static LabelStyle IconAndText => new LabelStyle(label =>
        {
            if (label.HasImage()) label.image.gameObject.SetActive(true);
            if (label.HasText()) label.text.gameObject.SetActive(true);
        });
        
        public static LabelStyle Hidden => new LabelStyle(label =>
        {
            if (label.HasImage()) label.image.gameObject.SetActive(false);
            if (label.HasText()) label.text.gameObject.SetActive(false);
        });
        
        // =============================================================== //
        //                        Implementation                           //
        // =============================================================== //
        public LabelStyle(UnityAction<LabelComponent> builder) : base(builder) { }
        protected override LabelStyle Create(UnityAction<LabelComponent> builder)
        {
            return new LabelStyle(builder);
        }
    }
}