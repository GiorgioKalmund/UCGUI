using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    public abstract class AbstractStyle<T, TStyle>
        where T : MonoBehaviour
        where TStyle : AbstractStyle<T, TStyle>
    {
        protected UnityAction<T> Builder { get; }

        protected AbstractStyle(UnityAction<T> builder)
        {
            Builder = builder;
        }

        public void Apply(T component)
        {
            Builder(component);
        }

        public TStyle Expand(UnityAction<T> expansionStyle)
        {
            return Create(NewBuilder(expansionStyle));
        }

        private UnityAction<T> NewBuilder(UnityAction<T> expansionStyle)
        {
            return component =>
            {
                Builder(component);
                expansionStyle(component);
            };
        }

        protected abstract TStyle Create(UnityAction<T> builder);
    }
}
