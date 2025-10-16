using System.Collections.Generic;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's default ViewStack.
    /// <br></br>
    /// <br></br>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="Push"/> - Pushes a view to the stack.</description></item>.
    /// <item><description><see cref="Pop"/> - Pops a view from the stack.</description></item>.
    /// <item><description><see cref="PopUntil"/> - Pops until the given view is on top of the stack.</description></item>.
    /// <item><description><see cref="Peek"/> - Returns the <see cref="ViewComponent"/> on top of the stack.</description></item>.
    /// <item><description><see cref="Collapse"/> - <see cref="Pop"/>s <b>all</b> views from the stack.</description></item>.
    /// </list>
    /// </summary>
    public class ViewStackComponent : BaseComponent
    {
        public Stack<ViewComponent> stack = new Stack<ViewComponent>();
        public void Start()
        {
            DisplayName = "ViewStack";
        }

        /// <summary>
        /// Removes the top view from the Stack when possible and closes it.
        /// </summary>
        ///
        /// <remarks>
        /// <i><see cref="ViewComponent.OnStackLeft"/> is invoked on the view here.</i>
        /// </remarks>
        public virtual void Pop()
        {
            if (stack.TryPop(out ViewComponent top))
                top.OnStackLeft().Close();
            else
                UCGUILogger.LogWarning("Cannot go back on already empty stack!");
        }

        /// <summary>
        /// Goes back to a specific view in the stack if present. If the target view is not part of the stack will not do anything.
        /// </summary>
        /// <param name="viewComponent">The view to go back to.</param>
        public virtual void PopUntil(ViewComponent viewComponent)
        {
            ViewComponent current = Peek();
            if (!stack.Contains(viewComponent))
            {
                UCGUILogger.LogWarning($"ViewStack does not contain {viewComponent}. Cannot go back to it!");
                return;
            }
            while (current != viewComponent && stack.Count != 0)
            {
                Pop();
                PopUntil(viewComponent);
                current = Peek();
            }
        }
        
        /// <summary>
        /// Pushes a view to the stack and opens it.
        /// </summary>
        /// <param name="viewComponent">The view to add to the stack.</param>
        ///
        /// <remarks>
        /// <i><see cref="ViewComponent.OnStackJoined"/> is invoked on the view here to allow it to hold a reference to the stack it is in.</i>
        /// </remarks>
        public virtual void Push(ViewComponent viewComponent)
        {
            if (!viewComponent)
                return;
            if (stack.Contains(viewComponent))
            {
                UCGUILogger.LogWarning($"ViewStack already contains {viewComponent}. Cannot forward to it again!");
                return;
            }
            
            stack.Push(viewComponent); 
            viewComponent.OnStackJoined(this).Open();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The top view in the stack without removing it.</returns>
        public ViewComponent Peek()
        {
            return stack.TryPeek(out ViewComponent top) ? top : null;
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
    }
}