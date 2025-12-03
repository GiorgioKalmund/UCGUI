using UnityEngine;

namespace UCGUI
{
    public abstract partial class SimpleScreen : BaseComponent
    {
        [HideInInspector]
        public Canvas canvas;
        
        public override void Awake()
        {
            base.Awake();
            canvas = GetCanvas();
            Create();
        }

        public virtual void Start()
        {
            this.SafeDisplayName("SimpleScreen");
            Initialize();
            if (!canvas)
            {
                UCGUILogger.LogWarning($"{DisplayName} (Start): No canvas bound to screen!");
            }
            this.Maximize();
        }

        /// <summary>
        /// Called during the 'Awake' phase of the Unity lifecycle. Build and configure all of your element in here.
        /// </summary>
        public abstract void Create();
        /// <summary>
        /// Called during the 'Start' phase of the Unity lifecycle. Do anything which needs to be done during 'Start' here!
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// Returns the canvas this screen refers to. 
        /// </summary>
        public abstract Canvas GetCanvas();
        
        /// <summary>
        /// Sets the <see cref="canvas"/> property of the screen. 
        /// </summary>
        /// <param name="c">The canvas to set.</param>
        /// <remarks>By default, <see cref="canvas"/> is initialized via <see cref="GetCanvas"/>. Please properly implement this method to assign <see cref="canvas"/>.</remarks>
        public SimpleScreen SetCanvas(Canvas c) 
        {
            canvas = c;
            return this;
        }
    }
}