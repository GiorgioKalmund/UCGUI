using JetBrains.Annotations;
using UnityEngine;

namespace UCGUI.Services
{
    public partial class ImageService
    {
        public static string MissingTextureLocation => Defaults.Services.MissingTexture2DLocation;
        public static readonly Sprite MissingSprite = GetSprite(MissingTextureLocation);
        
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
        
        public static Sprite GetSprite(string path, string resourceFolder = "Textures/")
        {
            if (string.IsNullOrEmpty(path))
            {
                UCGUILogger.LogWarning("Could not get image. Path empty or null.");
                return MissingSprite;
            }
            Texture2D texture = GetTexture2D(resourceFolder + path);
            return texture.ToSprite();
        }
        
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