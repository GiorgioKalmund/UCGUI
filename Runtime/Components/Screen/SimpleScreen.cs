using UnityEngine;

namespace UCGUI
{
    public abstract partial class SimpleScreen : BaseComponent
    {
        public Canvas canvas;
        protected Transform CanvasParent;
        public override void Awake()
        {
            base.Awake();

            canvas = GetCanvas();
            CanvasParent = canvas?.gameObject.transform;
        }

        private void Start()
        {
            this.SafeDisplayName("SimpleScreen");
            Setup();
            if (!canvas)
            {
                Debug.LogWarning("SimpleScreen (Awake): No canvas bound to screen!");
            }
            this.FullScreen(canvas);
        }

        public abstract void Setup();
        public abstract Canvas GetCanvas();
        
        public SimpleScreen SetCanvas(Canvas c) 
        {
            canvas = c;
            return this;
        }
    }
}