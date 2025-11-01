using JetBrains.Annotations;
using UCGUI.Services;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's Image Component.
    /// Noteworthy formatting functions:
    /// <list type="bullet">
    /// <item><description><see cref="Sprite(UnityEngine.Sprite,UnityEngine.UI.Image.Type?,float)"/> - Sets the image's sprite with optional <see cref="Image.Type"/> and <see cref="Image.pixelsPerUnitMultiplier"/>.</description></item>
    /// <item><description><see cref="GraphicComponent{T}.Color(UnityEngine.Color,bool)"/> - Sets the color. Allows you to also keep the previous alpha value if adding 'true'.</description></item>
    /// <item><description><see cref="GraphicComponent{T}.Alpha"/> - Sets the alpha.</description></item>
    /// <item><description><see cref="Clear"/> - Sets the sprite to null.</description></item>
    /// <item><description><see cref="NativeSize()"/> - Sets the RectTransform's size to the pixel size of the sprite. Use <see cref="NativeSize(Vector2)"/> or <see cref="NativeSize(float, float)"/> to additionally scale the image by a factor.</description></item>
    /// <item><description><see cref="ImageType"/> - <see cref="Image.Type"/> used to render the image.</description></item>.
    /// <item><description><see cref="Filled"/> - Allows you to specify a fill method, as well an optional fill amount. (Defaults to '1f').</description></item>.
    /// <item><description><see cref="FillAmount"/> - Adjusts the fill amount.</description></item>.
    /// <item><description><see cref="PixelsPerUnitMultiplier"/> - Adjusts the <see cref="Image.pixelsPerUnitMultiplier"/> of the image. For example when using <see cref="Image.Type.Sliced"/>.</description></item>.
    /// <item><description><see cref="GraphicComponent{T}.RaycastTarget"/> - Determines whether the image acts a raycast target.</description></item>.
    /// <item><description><see cref="GraphicComponent{T}.ToggleVisibility"/> - Enables / disables the image render.</description></item>.
    /// <item><description><see cref="AddAnimator"/> - Adds a <see cref="SpriteAnimator"/> behaviour.</description></item>.
    /// <item><description><see cref="Material"/> - Sets the material.</description></item>.
    /// </list>
    /// <para>
    /// Also implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    /// </summary>
    public partial class ImageComponent : GraphicComponent<ImageComponent>, ICopyable<ImageComponent>
    {
        private Image _image;
        protected static string NamePrefix = "ImageComponent";
        protected readonly static Vector2 DefaultSize = new Vector2(100, 100);
        
        private InputAction _toggleInputAction;
        
        protected SpriteAnimator Animator;

        public override void Awake()
        {
            base.Awake();
            _image = gameObject.GetOrAddComponent<Image>();
            this.Size(DefaultSize);
        }

        public virtual void Start()
        {
            this.SafeDisplayName(NamePrefix);

            if (_toggleInputAction != null)
                _toggleInputAction.performed += _ => ToggleVisibility();
        }

        public ImageComponent Sprite([CanBeNull] Sprite sprite, Image.Type? imageType = null, float pixelsPerUnitMultiplier = 1f) 
        {
            _image.sprite = sprite;
            // Only set if not already set from somewhere else
            this.SafeDisplayName(NamePrefix + ": " + (sprite?.name ?? "null"));
            if (imageType.HasValue)
                ImageType(imageType.Value);
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

        public Image GetImage()
        {
            return _image;
        }

        public override Graphic GetGraphic()
        {
            return _image;
        }

        public bool HasImage()
        {
            return _image.sprite;
        }

        public new ImageComponent Copy(bool fullyCopyRect = true)
        {
            ImageComponent copyImage = this.BaseCopy(this);
            return copyImage.CopyFrom(this, fullyCopyRect);
        }

        public override ImageComponent CopyFrom(ImageComponent other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            CopyImageProperties(other.GetImage(), this);
            CopyMaterial(other, this);
            CopyAnimator(other, this);
            return this;
        }
        
        public static void CopyImageProperties(Image image, ImageComponent copyImage) 
        {
            copyImage.ImageType(image.type);
            copyImage.Sprite(image.sprite);
            copyImage.PixelsPerUnitMultiplier(image.pixelsPerUnitMultiplier);
        }

        public static void CopyMaterial(ImageComponent other, ImageComponent copyImage)
        {
            var defaultMat = Graphic.defaultGraphicMaterial;
            if (other.GetImage().material && other.GetImage().material != defaultMat)
            {
                copyImage.Material(other.GetImage().material);
            }
        }

        public static void CopyAnimator(ImageComponent other, ImageComponent copyImage)
        {
            if (other.Animator)
            {
                copyImage.AddAnimator();
                copyImage.Animator.CopyFrom(other.Animator);
            }
        }

        public ImageComponent ToggleVisibilityUsing(InputAction action)
        {
            _toggleInputAction = action;
            _toggleInputAction?.Enable();
            return this;
        }

        private void OnEnable()
        {
            _toggleInputAction?.Enable();
        }

        private void OnDisable()
        {
            _toggleInputAction?.Disable();
        }

        // For more flexible and efficient use, allowing controlling of instances, we might want to use MaterialPropertyBlock in the future
        public ImageComponent Material(Material material)
        {
            _image.material = material;
            return this;
        }

        /// <summary>
        /// Creates an <see cref="SpriteAnimator"/> and adds it to the object. If one is already present, it will 
        /// return the existing.
        /// </summary>
        /// <returns><see cref="Animator"/></returns>
        public SpriteAnimator AddAnimator()
        {
            Animator = gameObject.GetOrAddComponent<SpriteAnimator>();
            Animator.DisplayName(DisplayName);
            return Animator;
        }

        public SpriteAnimator GetAnimator()
        {
            return Animator;
        }
    }

}