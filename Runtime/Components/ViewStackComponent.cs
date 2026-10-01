using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default ViewStack.
    /// </summary>
    public class ViewStackComponent : BaseComponent
    {
        protected ViewStackComponent() {}
        
        #region Events

        #region OnFirstPush 

        private UnityEvent _onFirstPush;

        /// <summary>
        /// Event fired whenever the stack becomes non-empty for the first time.
        /// </summary>
        public UnityEvent OnFirstPush {
            get
            {
                _onFirstPush ??= new UnityEvent();
                return _onFirstPush;
            }
            protected set => _onFirstPush = value;
        }

        #endregion

        #region OnLastPop

        private UnityEvent _onLastPop;

        /// <summary>
        /// Event fired whenever the view is closed.
        /// </summary>
        public UnityEvent OnLastPop
        {
            get
            {
                _onLastPop ??= new UnityEvent();
                return _onLastPop;
            }
            protected set => _onLastPop = value;
        }

        #endregion
        
        #endregion
        
        public List<AbstractViewComponent> stack = new List<AbstractViewComponent>();

        protected override void Awake()
        {
            base.Awake();
            DisplayName = "ViewStack";
        }

        /// <summary>
        /// Removes the top view from the Stack when possible and closes it.
        /// </summary>
        ///
        /// <remarks>
        /// <i><see cref="OnLastPop"/> is invoked when the stack becomes empty after the pop.</i>
        /// <i><see cref="AbstractViewComponent.LeaveStack"/> is invoked on the view here.</i>
        /// <i><see cref="AbstractViewComponent.OnStackHide"/> is invoked on the popped view here.</i>
        /// <i><see cref="AbstractViewComponent.OnStackReveal"/> is invoked on the new top view here.</i>
        /// </remarks>
        public AbstractViewComponent Pop()
        {
            if (stack.Count > 0)
            {
                var top = stack[^1];
                top.LeaveStack().Close();
                stack.RemoveAt(stack.Count - 1);
                
                if (stack.Count > 0)
                {
                    var newTop = stack[^1];
                    newTop.onStackReveal?.Invoke();
                    newTop.onStatusChanged?.Invoke(ViewStatus.Revealed);
                    newTop.HandleViewStackReveal();
                }
                if (stack.Count == 0)
                    _onLastPop?.Invoke();

                return top;
            }

            UCGUILogger.LogWarning("Cannot pop from an already empty stack!");
            return null;
        }

        /// <summary>
        /// Attempts to remove the top view from the stack and close it.
        /// </summary>
        /// <remarks>See more details on functionality in <see cref="Pop"/>.</remarks>
        public bool TryPop(out AbstractViewComponent view)
        {
            if (stack.Count > 0)
            {
                view = Pop();
                return true;
            }

            view = null;
            return false;
        }

        /// <summary>
        /// Goes back to a specific view in the stack if present. If the target view is not part of the stack will not do anything.
        /// </summary>
        /// <param name="abstractViewComponent">The view to go back to.</param>
        public bool PopUntil(AbstractViewComponent abstractViewComponent)
        {
            AbstractViewComponent current = Peek();
            if (!stack.Contains(abstractViewComponent))
            {
                UCGUILogger.LogWarning($"ViewStack does not contain {abstractViewComponent}. Cannot go back to it!");
                return false;
            }
            while (current != abstractViewComponent && stack.Count != 0)
            {
                Pop();
                PopUntil(abstractViewComponent);
                current = Peek();
            }
            return true;
        }
        
        /// <summary>
        /// Pushes a view to the stack and opens it.
        /// </summary>
        /// <param name="abstractViewComponent">The view to add to the stack.</param>
        ///
        /// <remarks>
        /// <i><see cref="OnFirstPush"/> is invoked when the stack is empty before the push.</i>
        /// <i><see cref="AbstractViewComponent.JoinStack"/> is invoked on the view here to allow it to hold a reference to the stack it is in.</i>
        /// <i><see cref="AbstractViewComponent.OnStackHide"/> is invoked on the previous top view here.</i>
        /// <i><see cref="AbstractViewComponent.OnStackReveal"/> is invoked on the pushed view here.</i>
        /// </remarks>
        public void Push(AbstractViewComponent abstractViewComponent)
        {
            if (!abstractViewComponent)
            {
                UCGUILogger.LogError($"Cannot push to ViewStack \"{DisplayName}\". New element is null!");
                return;
            }
            if (stack.Contains(abstractViewComponent))
            {
                UCGUILogger.LogWarning($"ViewStack already contains {abstractViewComponent}. Cannot push it again!");
                return;
            }

            if (stack.Count > 0)
            {
                var oldTop = stack[^1];
                oldTop.onStackHide?.Invoke();
                oldTop.onStatusChanged?.Invoke(ViewStatus.Hidden);
                oldTop.HandleViewStackHide();
            }
            else
                _onFirstPush?.Invoke();
            stack.Add(abstractViewComponent); 
            abstractViewComponent.JoinStack(this).Open();
        }

        /// <summary>
        /// Tries to peek at the top of the view stack.
        /// </summary>
        /// <returns>The top view in the stack without removing it.</returns>
        public bool TryPeek(out AbstractViewComponent view)
        {
            if (stack.Count > 0)
            {
                view = stack[^1];
                return true;
            }

            view = null;
            return false;
        }

        /// <summary>
        /// Tries to peek at the top of the view stack.
        /// </summary>
        /// <returns>The top view in the stack without removing it.</returns>
        public AbstractViewComponent Peek()
        {
            return stack.Count > 0 ? stack[^1] : null;
        }

        /// <summary>
        /// Goes back to the root of the stack until no more views are open and the stack is empty.
        /// </summary>
        public void Collapse()
        {
            int size = stack.Count;
            for (var i = 0; i < size; i++)
                Pop();
        }

        /// <summary>
        /// Whether the ViewStack is empty, aka. there are currently no elements part of it.
        /// </summary>
        public bool IsEmpty()
        {
            return stack.Count == 0;
        }

        /// <summary>
        /// Shorthand for replacing the top most view with a new view. Closes the top view and then pushes the new view onto the stack.
        /// </summary>
        /// <param name="with">The new view to replace the previous top view.</param>
        public void ReplaceTop(AbstractViewComponent with)
        {
            if (IsEmpty())
                UCGUILogger.LogWarning("[ViewStackComponent]: ReplaceTop was invoked but the stack is empty. Pushed the element to the stack anyways. Consider calling 'Push' instead to remove the warning.", with);
            else if (Peek() == with)
            {
                UCGUILogger.LogWarning($"[ViewStackComponent]: ReplaceTop was invoked with '{with.name}' but it's already at the top of the stack.", with);
                return;
            }
            else Pop();
            
            Push(with);
        }
    }
}