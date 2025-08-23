using System.Collections.Generic;
using UnityEngine;

namespace UCGUI.Game
{
    public class WheelMenu : BaseComponent, IRenderable
    {
        public float radius = 100;

        protected List<BaseComponent> WheelContents = new List<BaseComponent>();
        public int Count => WheelContents.Count;

        private void Start()
        {
            Render();
        }

        public void Render()
        {
            for (int index = 0; index < Count; index ++)
            {
                WheelContents[index] 
                        .Rotation(index * (360f / Count))
                    ;
                WheelContents[index].GetRect().Translate(new Vector3(0, radius, 0), Space.Self);
            }
        }

        public WheelMenu AddContent(params BaseComponent[] content)
        {
            foreach (var baseComponent in content)
            {
                baseComponent.Parent(transform);
            }
            WheelContents.AddRange(content);
            return this;
        }
        
        public WheelMenu Radius(float spacing)
        {
            radius = spacing;
            return this;
        }
    }
}