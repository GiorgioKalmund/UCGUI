using UnityEngine;

namespace UCGUI
{
    public abstract partial class SimpleScreen : BaseComponent
    {
        public Canvas canvas;
        protected Transform canvasParent;
        public override void Awake()
        {
            base.Awake();

            canvas = GetCanvas();
            canvasParent = canvas?.gameObject.transform;
        }

        private void Start()
        {
            this.SafeDisplayName("SimpleScreen");
            Setup();
            if (!canvas)
            {
                Debug.LogWarning("SimpleScreen (Awake): No canvas bound to screen!");
            }
            this.Maximize();
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