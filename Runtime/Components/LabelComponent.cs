using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class LabelComponent : SwitchLayoutComponent<LabelComponent>, IStylable<LabelComponent, LabelStyle>, ICopyable<LabelComponent>
    {
        protected LabelComponent() {}
        
        protected ImageComponent _image;

        public ImageComponent image
        {
            get => _image ??= UI.N<ImageComponent>(transform, "Image").RaycastTarget(false).Parent(this);
            set => _image = value;
        }

        protected TextComponent _text;
        
        public TextComponent text
        {
            get => _text ??=  UI.N<TextComponent>(transform, "Text").Parent(this);
            set => _text = value;
        }

        public override void Awake()
        {
            base.Awake();
            MakeBody();
        }

        public override void Start()
        {
            base.Start();
            DisplayName = "Label";
        }

        protected virtual void MakeBody()
        {
            FitToContents();
            ReverseArrangement();
        }
        
        public LabelComponent Text(string t, TextComponent.TextMode mode = TextComponent.TextMode.Normal, Color? color = null)
        {
            text.Text(t, mode, color);
            return this;
        }
        
        /// <summary>
        /// Adds an <see cref="LayoutElement"/> to the image and sets 'minWidth' and 'minHeight' of
        /// its layout.
        /// </summary>
        /// <param name="minWidth">The minimum width the image will retain.</param>
        /// <param name="minHeight">The minium height the image will retain.</param>
        public LabelComponent ImageSize(float minWidth, float minHeight)
        {
            image.AddLayoutElement(minWidth, minHeight);
            return this;
        }

        /// <summary>
        /// Adds an <see cref="LayoutElement"/> to the image and sets 'minWidth' and 'minHeight' of
        /// its layout.
        /// </summary>
        /// <param name="size">The min width and height.</param>
        /// <seealso cref="ImageSize(float, float)"/>
        public LabelComponent ImageSize(Vector2 size) => ImageSize(size.x, size.y);

        public override LabelComponent FitToContents(bool fit = true)
        {
            base.FitToContents(fit);
            text.FitToContents(fit);
            return this;
        }
        
        public LabelComponent Style(LabelStyle style)
        {
            style.Apply(this);
            return this;
        }

        public new virtual LabelComponent Copy(bool fullyCopyRect = true)
        {
            LabelComponent copyLabel = this.BaseCopy(this);
            return copyLabel.CopyFrom(this, fullyCopyRect);
        }

        public new virtual LabelComponent CopyFrom(LabelComponent other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            if (other._text != null)
                text.CopyFrom(other.text, fullyCopyRect);
            if (other._image != null)
                image.CopyFrom(other.image, fullyCopyRect);
            return this;
        }

        public new bool HasImage()
        {
            return _image != null;
        }
        
        public bool HasText()
        {
            return _text != null;
        }

        public LabelComponent Init(string s, Sprite sprite = null)
        {
            text.Text(s);
            if (sprite != null)
                image.Sprite(sprite);
            return this;
        }

        protected override HorizontalOrVerticalLayoutGroup GetLayout()
        {
            if (VerticalLayout)
                return VerticalLayout;
            return HorizontalLayout;
        }
    }
}