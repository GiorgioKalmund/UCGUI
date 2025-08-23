using UnityEngine;
using UnityEngine.InputSystem;

namespace UCGUI
{
    public class ResizableWindowComponent : WindowComponent
    {
        
        // -- Subcomponents -- //
        protected WindowResizer WindowResizer;

        private bool _allowResize = true;

        public override void Awake()
        {
            base.Awake();

            WindowResizer = ComponentExtension.N<WindowResizer>(WindowBase)
                .Pivot(PivotPosition.LowerRight, true)
                .Size(40, 40)
                .Build(this)
                .Sprite("gui_assets" ,"resizer")
                .Cast<WindowResizer>()
                ;
        }

        public override void Start()
        {
            base.Start();
            this.SafeDisplayName("ResizableWindowComponent");
        }

        
        
        public ResizableWindowComponent Build(InputAction action, string title, Color windowColor, Color headerColor)
        {
            Build(title, action);
            WindowBase.Color(windowColor);
            Header.Color(headerColor);
            return this;
        }
        
        public ResizableWindowComponent Build(string title, Color windowColor, Color headerColor)
        {
            return Build(null ,title, windowColor, headerColor);
        }

        public override void Collapse()
        {
            base.Collapse();
            WindowResizer.SetActive(!_allowResize);
        }
        
        public override void Expand()
        {
            base.Expand();
            WindowResizer.SetActive(_allowResize);
        }

        public ResizableWindowComponent NoResize()
        {
            _allowResize = false;
            WindowResizer.SetActive(false);
            return this;
        }
    }
}