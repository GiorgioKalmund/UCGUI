using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UCGUI
{
    
    /// <summary>
    /// <para>
    /// Implements <see cref="IPointerEnterHandler"/> and <see cref="IPointerExitHandler"/> which allows any deriving classes to handle this by overriding <see cref="HandlePointerEnter"/> and <see cref="HandlePointerExit"/>>.
    /// </para>
    /// <para>
    /// Also implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The extending member itself.</typeparam>
    public abstract class GraphicComponent<T> : BaseComponent, IPointerEnterHandler, IPointerExitHandler, IEnabled where T : GraphicComponent<T>
    {
        
        public abstract Graphic GetGraphic();
        
        public virtual T CopyFrom(T other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            CopyProperties(other.GetGraphic(), this);
            return (T)this;
        }

        private void CopyProperties(Graphic graphic, GraphicComponent<T> graphicComponent)
        {
            graphicComponent.Color(graphic.color);
            graphicComponent.RaycastTarget(graphic.raycastTarget);
        }

        public T Color(Color color, bool keepPreviousAlphaValue = false)
        {
            var graphic = GetGraphic();
            if (keepPreviousAlphaValue)
            {
                var prevAlpha = graphic.color.a;
                graphic.color = color;
                Alpha(prevAlpha);
            }
            else
            {
                graphic.color = color;
            }
            return (T)this;
        }
        
        public T Color(Color color, float alpha)
        {
            return Color(color).Alpha(alpha);
        }
        
        public T Alpha(float alpha)
        {
            var graphic = GetGraphic();
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
            
            return (T)this;
        }
        
        public T RaycastTarget(bool target)
        {
            GetGraphic().raycastTarget = target;
            return (T)this;
        }
        
        public void ToggleVisibility()
        {
            GetGraphic().enabled = !GetGraphic().enabled;
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
        
        public virtual void Enabled(bool on)
        {
            GetGraphic().enabled = on;
        }
    }
}