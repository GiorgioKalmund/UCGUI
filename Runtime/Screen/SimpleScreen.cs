using UnityEngine;

namespace UCGUI
{
    public abstract class SimpleScreen : BaseComponent
    {
        [SerializeField] public Canvas canvas;
        
        protected override void Awake()
        {
            base.Awake();
            
            canvas ??= GetCanvas();
            
            BeginParentContext(this);
            Create();
            EndParentContext();
        }

        protected virtual void Start()
        {
            if (!canvas)
                UCGUILogger.LogWarning($"{DisplayName} (Start): No canvas bound to screen!", this);
            
            this.Maximize();
            
            BeginParentContext(this);
            Initialize();
            EndParentContext();
        }

        /// <summary>
        /// Called during the 'Awake' phase of the Unity lifecycle. Build and configure all of your elements in here.
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
    }
}