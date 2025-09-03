using UCGUI.Services;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UCGUI
{
    /// UCGUI's Image Component.
    /// Noteworthy formatting functions:
    /// <list type="bullet">
    /// <item><description><see cref="Sprite(UnityEngine.Sprite,UnityEngine.UI.Image.Type?,float)"/> - Sets the image's sprite with optional <see cref="Image.Type"/> and <see cref="Image.pixelsPerUnitMultiplier"/>.</description></item>
    /// <item><description><see cref="Color(UnityEngine.Color,bool)"/> - Sets the color. Allows you to also keep the previous alpha value if adding 'true'.</description></item>
    /// <item><description><see cref="Alpha"/> - Sets the alpha.</description></item>
    /// <item><description><see cref="ClearSprite"/> - Sets the sprite to null.</description></item>
    /// <item><description><see cref="NativeSize()"/> - Sets the RectTransform's size to the pixel size of the sprite. Use <see cref="NativeSize(Vector2)"/> or <see cref="NativeSize(float, float)"/> to additionally scale the image by a factor.</description></item>
    /// <item><description><see cref="ImageType"/> - <see cref="Image.Type"/> used to render the image.</description></item>.
    /// <item><description><see cref="Filled"/> - Allows you to specify a fill method, as well an optional fill amount. (Defaults to '1f').</description></item>.
    /// <item><description><see cref="FillAmount"/> - Adjusts the fill amount.</description></item>.
    /// <item><description><see cref="PixelsPerUnitMultiplier"/> - Adjusts the <see cref="Image.pixelsPerUnitMultiplier"/> of the image. For example when using <see cref="Image.Type.Sliced"/>.</description></item>.
    /// <item><description><see cref="RaycastTarget"/> - Determines whether the image acts a raycast target.</description></item>.
    /// <item><description><see cref="ToggleVisibility"/> - Enables / disables the image render.</description></item>.
    /// <item><description><see cref="AddAnimator"/> - Adds a <see cref="SpriteAnimator"/> behaviour.</description></item>.
    /// <item><description><see cref="Material"/> - Sets the material.</description></item>.
    /// </list>
    /// <para>
    /// Implements <see cref="IPointerEnterHandler"/> and <see cref="IPointerExitHandler"/> which allows any deriving classes to handle this by overriding <see cref="HandlePointerEnter"/> and <see cref="HandlePointerExit"/>>.
    /// </para>
    /// <para>
    /// Also implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    public class ImageComponent : BaseComponent, ICopyable<ImageComponent>, IPointerEnterHandler, IPointerExitHandler, IEnabled
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
                _toggleInputAction.performed += ToggleVisibility;
        }

        public ImageComponent Sprite(Sprite sprite, Image.Type? imageType = null, float pixelsPerUnitMultiplier = 1f) 
        {
            _image.sprite = sprite;
            // Only set if not already set from somewhere else
            this.SafeDisplayName(NamePrefix + ": " + sprite?.name);
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

        public ImageComponent ClearSprite()
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
        
        public ImageComponent Color(Color color, bool keepPreviousAlphaValue = false) 
        {
            if (keepPreviousAlphaValue)
            {
                var prevAlpha = _image.color.a;
                _image.color = color;
                Alpha(prevAlpha);
            }
            else
            {
                _image.color = color;
            }
            return this;
        }

        public ImageComponent Color(Color color, float alpha)
        {
            return Color(color).Alpha(alpha);
        }
        
        public ImageComponent Alpha(float alpha)
        {
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
            return this;
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

        public ImageComponent RaycastTarget(bool target)
        {
            _image.raycastTarget = target;
            return this;
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

        public ImageComponent CopyFrom(ImageComponent other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            //DisplayName = other.DisplayName + " (Copy)";
            CopyImageProperties(other.GetImage(), this);
            CopyAnimator(other, this);
            CopyMaterial(other, this);
            return this;
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

        public static void CopyImageProperties(Image image, ImageComponent copyImage) 
        {
            copyImage.ImageType(image.type);
            copyImage.Color(image.color);
            copyImage.Sprite(image.sprite);
            copyImage.PixelsPerUnitMultiplier(image.pixelsPerUnitMultiplier);
            copyImage.RaycastTarget(image.raycastTarget);
        }

        public ImageComponent ToggleVisibilityUsing(InputAction action)
        {
            _toggleInputAction = action;
            _toggleInputAction?.Enable();
            return this;
        }

        public void ToggleVisibility()
        {
            GetImage().enabled = !GetImage().enabled;
        }
        public void ToggleVisibility(InputAction.CallbackContext callbackContext)
        {
            ToggleVisibility();
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

        public SpriteAnimator AddAnimator()
        {
            Animator = gameObject.AddComponent<SpriteAnimator>();
            Animator.DisplayName(DisplayName);
            return Animator;
        }

        public SpriteAnimator GetAnimator()
        {
            return Animator;
        }

        public HorizontalLayoutGroup GetHorizontalLayout()
        {
            return HorizontalLayout;
        }
        
        
        public virtual void HandlePointerEnter(PointerEventData eventData) { }
        public virtual void HandlePointerExit(PointerEventData eventData) { }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            HandlePointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandlePointerExit(eventData);
        }

        public void Enabled(bool on)
        {
            _image.enabled = on;
        }
    }

}