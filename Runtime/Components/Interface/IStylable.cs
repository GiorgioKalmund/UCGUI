using UnityEngine;

namespace UCGUI
{
    public interface IStylable<T, TStyle>
        where T : MonoBehaviour
        where TStyle : AbstractStyle<T, TStyle>
    {
        /// <summary>
        /// Applies a given style to the implementing member.
        /// </summary>
        /// <param name="style">The <see cref="TStyle"/> to apply.</param>
        /// <returns>
        /// Instance of the member with the style applied.
        /// </returns>
        /// <seealso cref="AbstractStyle{T,TStyle}.Apply"/>
        public T Style(TStyle style);
    }
}