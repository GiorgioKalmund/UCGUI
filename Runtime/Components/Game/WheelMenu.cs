using System.Collections.Generic;
using UnityEngine;

namespace UCGUI.Game
{
    /// <summary>
    /// UCGUI's default Wheel Menu.
    /// Noteworthy formatting functions:
    /// <list type="bullet">
    /// <item><description><see cref="Radius"/> - Sets the radius.</description></item>
    /// <item><description><see cref="Add"/> - Adds a component to the wheel.</description></item>
    /// </list>
    /// <para>
    /// Also implements <see cref="IRenderable"/> which allows <see cref="IRenderable.Render"/>.
    /// </para>
    /// </summary>
    public class WheelMenu : BaseComponent, IRenderable
    {
        public float radius = 100;

        protected List<BaseComponent> WheelContents = new List<BaseComponent>();
        public int Count => WheelContents.Count;

        private void Start()
        {
            Render();
        }

        /// <summary>
        /// Renders the wheel menu itself. In this stage all the content is positioned based on <see cref="Radius"/>.
        /// Automatically called once on <see cref="Start"/>.
        /// </summary>
        public void Render()
        {
            for (int index = 0; index < Count; index ++)
            {
                WheelContents[index] 
                        .Rotation(index * (360f / Count))
                    ;
                WheelContents[index].Pos(new Vector3(0, radius, 0), Space.Self);
            }
        }

        /// <summary>
        /// Adds any component to the contents of the wheel. Will be spread out equidistanly automatically (based on <see cref="Radius"/>).
        /// </summary>
        /// <param name="content">
        /// The content to add.
        /// </param>
        /// <returns></returns>
        public WheelMenu Add(params BaseComponent[] content)
        {
            foreach (var baseComponent in content)
            {
                baseComponent.Parent(transform);
            }
            WheelContents.AddRange(content);
            return this;
        }
        
        /// <summary>
        /// Sets the radius of the Wheel Menu
        /// </summary>
        /// <param name="spacing">
        /// The amount of spacing
        /// </param>
        /// <returns></returns>
        public WheelMenu Radius(float spacing)
        {
            radius = spacing;
            return this;
        }

        /// <summary>
        /// Builder for UCGUI.Game <see cref="WheelMenu"/>.<br/>
        /// <b>Builder Functions:</b>
        /// <list type="bullet">
        /// <item><description><see cref="Add"/> - Adds a component to the wheel.</description></item>
        /// </list>
        /// </summary>
        public class Builder
        {
            private WheelMenu _menu;

            public Builder(WheelMenu menu)
            {
                _menu = menu;
            }

            public void Add(params BaseComponent[] contents)
            {
                _menu.Add(contents);
            }
        }
    }
}