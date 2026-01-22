using UnityEngine;

namespace UCGUI.Services
{
    /// <summary>
    /// A simple interface for loading Materials using Unity's Resources API
    /// </summary>
    public partial class MaterialService
    {
        
        /// <summary>
        /// Loads a given path as a <see cref="Material"/> using Unity's Resources API
        /// </summary>
        /// <param name="path">The path to load the material from.</param>
        /// <param name="resourceFolder">Parent folder of the path. Is prefix-appended to the path. Defaults to `Materials`.</param>
        /// <returns>
        /// The sprite as a Sprite if the path is valid and can be resolved, else null.
        /// </returns>
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