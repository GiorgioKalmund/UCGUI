using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    /// <summary>
    /// The SpacerComponent can act as a greedy space allocator inside of layouts to maximally spread elements apart.
    /// <br></br>
    /// <br></br>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="SetBehaviour"/> - Sets a custom <see cref="ISpacerBehaviour"/> which is going to be applied during the 'Start' phase.</description></item>
    /// <item><description><see cref="ReplaceBehaviour"/> - Replaces the current <see cref="ISpacerBehaviour"/> and applies it.</description></item>
    /// <item><description><see cref="AutoSetBehaviour"/> - Automatically determines the correct behaviour in the current context and sets it.</description></item>
    /// <item><description><see cref="Render"/> - Applies the <see cref="_behaviour"/> after one frame.</description></item>
    /// <item><description><see cref="RenderImmediate"/> - Immediately applies the <see cref="_behaviour"/>.</description></item>
    /// <item><description><see cref="Enabled"/> - Sets the active state of the game object.</description></item>
    /// </list>
    /// </summary>
    public class SpacerComponent : BaseComponent, IEnabled, IRenderable
    {
        private ISpacerBehaviour _behaviour;

        public virtual void Start()
        {
            this.SafeDisplayName("Spacer");
            
            if (_behaviour == null)
                AutoSetBehaviour();
            
            if (_behaviour == null)
            {
                UCGUILogger.LogError("Spacer does not have a behaviour attached! Please specify one!");
                return;
            }

            Render();
        }

        /// <summary>
        /// Sets the internal <see cref="ISpacerBehaviour"/>.
        /// </summary>
        /// <param name="behaviour">The behaviour to set.</param>
        public SpacerComponent SetBehaviour(ISpacerBehaviour behaviour)
        {
            if (behaviour == null)
            {
                UCGUILogger.LogError($"ISpacerBehaviour cannot be null!");
                return this;
            }

            if (_behaviour != null)
            {
                UCGUILogger.LogError($"Spacer does already a behaviour attached! Cannot additionally add {behaviour.GetType()}\nIf you really want this Spacer to have a different behaviour please use 'ReplaceBehaviour' instead.");
                return this;
            }

            _behaviour = behaviour;
            return this;
        }

        /// <summary>
        /// Replaces the currently active behaviour and applies it.
        /// </summary>
        /// <param name="behaviour">The behaviour to replace the currently active one.</param>
        public void ReplaceBehaviour(ISpacerBehaviour behaviour)
        {
            if (behaviour == null)
            {
                UCGUILogger.LogError($"ISpacerBehaviour cannot be null!");
                return;
            }
            
            _behaviour = behaviour;
            _behaviour.Apply(this);
        }

        /// <summary>
        /// Automatically sets the behaviour based on the context the Spacer is in.
        /// </summary>
        /// <param name="overrideExisting">If set to true, allows to override any existing behaviour. If not set, will not allow setting a new behaviour if one is already present.</param>
        public void AutoSetBehaviour(bool overrideExisting = false)
        {
            if (!overrideExisting && _behaviour != null)
            {
                UCGUILogger.LogError("An existing behaviour already attached. Cannot auto set. Set 'overrideExisting' to true to set a new behaviour.");
                return;
            }
            
            if (gameObject.GetComponentInParent<HStackComponent>())
                SetBehaviour(new HorizontalSpacerBehaviour());
            else if (gameObject.GetComponentInParent<VStackComponent>())
                SetBehaviour(new VerticalSpacerBehaviour());
            else
            {
                UCGUILogger.LogError("Spacer: Cannot auto set behaviour based on parent. Is the spacer part of a valid hierarchy?");
            }
        }

        /// <summary>
        /// Sets the active state of the game object.
        /// </summary>
        /// <param name="on">Boolean controlling the active state.</param>
        public void Enabled(bool on)
        {
            gameObject.SetActive(on);
        }

        /// <summary>
        /// Renders the Spacer by calling the behaviour's <see cref="ISpacerBehaviour.Apply"/> function one frame later to allow all sub-layouts to first properly compute and build.
        /// </summary>
        public void Render()
        {
            // We wait here to allow all sub-hierarchies and layouts to also build beforehand, otherwise the 
            // calculated spacing might be incorrect
            StartCoroutine(RenderRoutine());
        }

        /// <summary>
        /// Immediately applies the attached <see cref="ISpacerBehaviour"/>.
        /// </summary>
        public void RenderImmediate()
        {
            _behaviour.Apply(this);
        }

        /// <summary>
        /// Coroutine to render the Spacer one frame later. This is especially helpful to avoid layout inconsistencies due to Unity's layout hierarchy building.
        /// </summary>
        private IEnumerator RenderRoutine()
        {
            yield return new WaitForEndOfFrame();
            RenderImmediate();
        }
    }
}