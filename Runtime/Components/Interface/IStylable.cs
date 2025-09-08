using UnityEngine;

namespace UCGUI
{
    public interface IStylable<T, TStyle>
        where T : MonoBehaviour
        where TStyle : AbstractStyle<T, TStyle>
    {
        public T Style(TStyle style);
    }
}