using UnityEngine;

namespace UCGUI.Services
{
    
    public partial class ImageService
    {
        public static readonly string MissingTextureLocation = "Textures/missing";
        public static readonly Sprite MissingSprite = GetSprite(MissingTextureLocation);
        public static Sprite White => GetSprite("white");
        public static Texture2D GetTexture2D(string path, FilterMode filterMode = FilterMode.Point)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Resources.Load<Texture2D>(MissingTextureLocation);
            }
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (!texture)
            {
                //Debug.LogWarning("No image found for: " + path);
                texture = Resources.Load<Texture2D>(MissingTextureLocation);
            }
            texture.filterMode = filterMode;
            return texture;
        }
    
        public static Sprite GetSpriteFromAsset(string asset, string spriteName)
        {
            var sprites = Resources.LoadAll<Sprite>("Aseprite/" + asset);
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
                Debug.LogWarning("No image found for: Aseprite/" + asset + ": " + spriteName);
                return GetSprite("missing");
            }

            return toReturn;
        }
    
        public static Sprite GetSpriteDirectly(string path, string resourceFolder = "Textures/")
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Could not get image. Path empty.");
                return GetSprite(MissingTextureLocation);
            }
            Sprite sprite = Resources.Load<Sprite>(resourceFolder + path);
            if (!sprite)
            {
                Debug.LogWarning("Could not get image:" + resourceFolder + path);
                return GetSprite(MissingTextureLocation);
            }
            return sprite;
        }
    
        public static Sprite GetSprite(string path, string resourceFolder = "Textures/")
        {
            Texture2D texture = GetTexture2D(resourceFolder + path);
            return texture.ToSprite();
        }
    }
}