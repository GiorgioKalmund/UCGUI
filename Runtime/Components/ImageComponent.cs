using JetBrains.Annotations;
using UCGUI.Services;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's Image Component.
    /// <para>
    /// Implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    /// </summary>
    public class ImageComponent : GraphicComponent<ImageComponent>, ICopyable<ImageComponent>, IStylable<ImageComponent, ImageStyle>
    {
        protected ImageComponent() {}
        
        private Image _image;
        protected static readonly string NamePrefix = "Image";
        
        protected SpriteAnimator animator;

        protected override void Awake()
        {
            base.Awake();
            DisplayName = NamePrefix;
            
            _image = gameObject.GetOrAddComponent<Image>();
            this.Size(Defaults.Image.DefaultSize);
        }

        /// <summary>
        /// Generates an appropriate name for the component and updates it in the editor.
        /// </summary>
        public void GenerateName()
        {
            DisplayName = NamePrefix + $": {_image.sprite.name}";
        }

        public ImageComponent Sprite([CanBeNull] Sprite sprite, Image.Type? imageType = null, float pixelsPerUnitMultiplier = -1f) 
        {
            _image.sprite = sprite;
            if (imageType.HasValue)
                ImageType(imageType.Value);
            if (pixelsPerUnitMultiplier > 0)
                PixelsPerUnitMultiplier(pixelsPerUnitMultiplier);
            return this;
        }
        
        public ImageComponent Sprite(Texture2D texture2D)
        {
            return Sprite(texture2D.ToSprite());
        }
        
        public ImageComponent Sprite(string path, Image.Type? imageType = null, bool direct = false)
        {
            return direct ? Sprite(ImageService.GetSpriteDirectly(path), imageType) : Sprite(ImageService.GetSprite(path), imageType) ;
        }
        
        public ImageComponent Sprite(string asset, string layerName, Image.Type? imageType = null)
        {
            return Sprite(ImageService.GetSpriteFromAsset(asset, layerName), imageType);
        }

        public ImageComponent Clear()
        {
            return Sprite((Sprite)null);
        }

        public ImageComponent NativeSize(Vector2 scale)
        {
            return this.Size(_image.sprite.NativeSize() * scale);
        }
        public ImageComponent NativeSize()
        {
            return NativeSize(Vector2.one);
        }
        public ImageComponent NativeSize(float scaleFactorX, float scaleFactorY)
        {
            return NativeSize(new Vector2(scaleFactorX, scaleFactorY));
        }

        public Vector2 GetNativeSize()
        {
            return _image.sprite.NativeSize();
        }
        
        public ImageComponent ImageType(Image.Type imageType)
        {
            _image.type = imageType;
            return this;
        }

        public ImageComponent Filled(Image.FillMethod method, float amount = 1f)
        {
            ImageType(Image.Type.Filled);
            _image.fillMethod = method;
            FillAmount(amount);
            return this;
        }

        public ImageComponent FillAmount(float amount)
        {
            _image.fillAmount = amount;
            return this;
        }
        
        public ImageComponent PixelsPerUnitMultiplier(float multiplier)
        {
            _image.pixelsPerUnitMultiplier = multiplier;
            return this;
        }
        
        public ImageComponent Maskable(bool maskable)
        {
            _image.maskable = maskable;
            return this;
        }

        public Image GetImage() { return _image; }

        public override Graphic GetGraphic() { return _image; }

        public bool HasImage() { return _image.sprite; }

        public new ImageComponent Copy(bool fullyCopyRect = true)
        {
            ImageComponent copyImage = this.BaseCopy(this);
            return copyImage.CopyFrom(this, fullyCopyRect);
        }

        public override ImageComponent CopyFrom(ImageComponent other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            CopyImageProperties(other.GetImage(), this);
            CopyAnimator(other, this);
            return this;
        }
        
        public static void CopyImageProperties(Image image, ImageComponent copyImage) 
        {
            copyImage.ImageType(image.type);
            copyImage.Sprite(image.sprite);
            copyImage.PixelsPerUnitMultiplier(image.pixelsPerUnitMultiplier);
        }

        public static void CopyAnimator(ImageComponent other, ImageComponent copyImage)
        {
            if (other.animator)
            {
                copyImage.AddAnimator();
                copyImage.animator.CopyFrom(other.animator);
            }
        }

        /// <summary>
        /// Creates an <see cref="SpriteAnimator"/> and adds it to the object. If one is already present, it will 
        /// return the existing.
        /// </summary>
        /// <returns><see cref="animator"/></returns>
        public SpriteAnimator AddAnimator()
        {
            animator = gameObject.GetOrAddComponent<SpriteAnimator>();
            animator.DisplayName(DisplayName);
            return animator;
        }

        public SpriteAnimator GetAnimator()
        {
            return animator;
        }

        public ImageComponent Style(ImageStyle style)
        {
            style.Apply(this);
            return this;
        }
    }

}