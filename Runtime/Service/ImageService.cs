using JetBrains.Annotations;
using UnityEngine;

namespace UCGUI.Services
{
    /// <summary>
    /// The ImageService uses Unity's Resources API to load
    /// Texture2D and Sprite images.
    /// </summary>
    public partial class ImageService
    {
        /// <summary>
        /// Path to a texture(2d) which should be displayed if a path is unavailable.
        /// </summary>
        public static string MissingTextureLocation => Defaults.Services.MissingTexture2DLocation;
        
        /// <summary>
        /// Sprite version of <see cref="MissingTextureLocation"/>
        /// </summary>
        public static readonly Sprite MissingSprite = GetSprite(MissingTextureLocation);
        
        /// <summary>
        /// Loads a given path as a <see cref="Texture2D"/> using Unity's Resources API
        /// </summary>
        /// <param name="path">The path to load the texture from.</param>
        /// <param name="filterMode">Optional <see cref="FilterMode"/> applied to the texture. Defaults to <see cref="FilterMode.Point"/>.</param>
        /// <returns>
        /// The image as a Texture2D if the path is valid and can be resolved, else the texture under <see cref="MissingTextureLocation"/>.
        /// </returns>
        [CanBeNull]
        public static Texture2D GetTexture2D(string path, FilterMode filterMode = FilterMode.Point)
        {
            if (string.IsNullOrEmpty(path))
            {
                UCGUILogger.LogWarning("Could not get image. Path empty or null.");
                return Resources.Load<Texture2D>(MissingTextureLocation);
            }
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (!texture)
            {
                UCGUILogger.LogError($"Resource loading error! Not Texture2D found at '{path}'");
                texture = Resources.Load<Texture2D>(MissingTextureLocation);
                return texture;
            }
            texture.filterMode = filterMode;
            return texture;
        }
        
        /// <summary>
        /// Loads a given path as a <see cref="Sprite"/> using Unity's Resources API.
        /// Initially loads the image as a Texture2D, then converts it to a Sprite.
        /// </summary>
        /// <param name="path">The path to load the texture from.</param>
        /// <param name="resourceFolder">Parent folder of the path. Is prefix-appended to the path. Defaults to `Textures`.</param>
        /// <returns>
        /// The image as a Sprite if the path is valid and can be resolved,
        /// if the path is empty returns <see cref="MissingSprite"/>,
        /// else if the path is null, returns null.
        /// </returns>
        /// <example>
        /// <code>
        /// // Returns the texture under "Assets/Resources/Textures/hello(.png)"
        /// 
        /// Sprite sprite = ImageService.GetSprite("hello");
        /// </code>
        /// </example>
        /// <remarks>If you are attempting to load a Sprite directly, for example to make use of 9-slicing, use <see cref="GetSpriteDirectly"/>.</remarks>
        public static Sprite GetSprite(string path, string resourceFolder = "Textures/")
        {
            if (path == "")
            {
                UCGUILogger.LogWarning("Could not get image. Path empty.");
                return MissingSprite;
            }
            if (path == null)
                return null;
            
            Texture2D texture = GetTexture2D(resourceFolder + path);
            return texture.ToSprite();
        }
        
        /// <summary>
        /// Loads a given path as a <see cref="Sprite"/> using Unity's Resources API
        /// </summary>
        /// <param name="path">The path to load the sprite from.</param>
        /// <param name="resourceFolder">Parent folder of the path. Is prefix-appended to the path. Defaults to `Textures`.</param>
        /// <returns>
        /// The sprite as a Sprite if the path is valid and can be resolved, else <see cref="MissingSprite"/>.
        /// </returns>
        public static Sprite GetSpriteDirectly(string path, string resourceFolder = "Textures/")
        {
            if (string.IsNullOrEmpty(path))
            {
                UCGUILogger.LogWarning("Could not get image. Path empty or null.");
                return MissingSprite;
            }
            Sprite sprite = Resources.Load<Sprite>(resourceFolder + path);
            if (!sprite)
            {
                UCGUILogger.LogError($"Resource loading error! Not Sprite found at '{path}'");
                return MissingSprite;
            }
            return sprite;
        }
    
        /// <summary>
        /// === EXPERIMENTAL ===
        /// Loads a Sprite from a given SpriteAtlas asset.
        /// ====================
        /// </summary>
        /// <param name="asset">The name of the asset.</param>
        /// <param name="spriteName">The name of the layer / sprite.</param>
        /// <returns>
        /// The sprite as a Sprite if the path is valid and can be resolved, else <see cref="MissingSprite"/>.
        /// </returns>
        public static Sprite GetSpriteFromAsset(string asset, string spriteName)
        {
            const string root = "Aseprite/"; // TODO
            
            var sprites = Resources.LoadAll<Sprite>(root + asset);
            Sprite toReturn = null;
            foreach (var sprite in sprites)
            {
                if (sprite.name == spriteName)
                {
                    toReturn = sprite;
                    break;
                }
            }

            if (!toReturn)
            {
                UCGUILogger.LogError($"No image found for: {root}" + asset + ": " + spriteName);
                return MissingSprite;
            }
            return toReturn;
        }
    }
}