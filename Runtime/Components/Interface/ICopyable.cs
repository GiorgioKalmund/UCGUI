using UnityEngine;

namespace UCGUI
{
    public partial interface ICopyable<T> where T : MonoBehaviour
    {
        /// <summary>
        /// Returns a copy of the calling object.
        /// </summary>
        /// <param name="fullyCopyRect">
        /// Whether to copy the full RectTransform, or only parts of it. Context: <see cref="BaseComponent.CopyRect"/>
        /// </param>
        /// <remarks>Often based on <see cref="CopyFrom"/> under the hood.</remarks>
        /// <returns></returns>
        T Copy(bool fullyCopyRect = true);
        /// <summary>
        /// Copies the other <see cref="ICopyable{T}"/> onto the caller.
        /// </summary>
        /// <param name="other">
        /// The object to copy behaviour, values, etc. from.
        /// </param>
        /// <param name="fullyCopyRect">
        /// Whether to copy the full RectTransform, or only parts of it. Context: <see cref="BaseComponent.CopyRect"/>
        /// </param>
        /// <returns>
        /// </returns>
        T CopyFrom(T other, bool fullyCopyRect = true);
    }
    
    public static class CopyableExtensions
    {
        public static T BaseCopy<T>(this BaseComponent src, T component) where T : BaseComponent
        {
            GameObject copy = UI
                .CreateEmptyGameObjectWithParent(
                    component.GetParent(),
                    false,
                    component.DisplayName
                );
            var copyComponent = copy.AddComponent<T>();

            return copyComponent;
        }

    }
}