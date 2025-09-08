using UnityEngine;

namespace UCGUI.Services
{
    public partial class MaterialService
    {
        public static Material GetMaterial(string path, string resourceFolder = "Materials/")
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Could not get material. Path empty.");
                return null;
            }
            Material material = Resources.Load<Material>(resourceFolder + path);
            return material;
        }
    }
}