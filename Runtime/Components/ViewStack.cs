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
    /// <item><description><see cref="Peek"/> - Returns the <see cref="View"/> on top of the stack.</description></item>.
    /// <item><description><see cref="Collapse"/> - <see cref="Pop"/>s <b>all</b> views from the stack.</description></item>.
    /// </list>
    /// </summary>
    public class ViewStack : BaseComponent
    {
        public Stack<View> stack = new Stack<View>();
        public void Start()
        {
            DisplayName = "ViewStack";
        }

        /// <summary>
        /// Removes the top view from the Stack when possible and closes it.
        /// </summary>
        ///
        /// <remarks>
        /// <i><see cref="View.OnStackLeft"/> is invoked on the view here.</i>
        /// </remarks>
        public virtual void Pop()
        {
            if (stack.TryPop(out View top))
                top.OnStackLeft().Close();
            else
                UCGUILogger.LogWarning("Cannot go back on already empty stack!");
        }

        /// <summary>
        /// Goes back to a specific view in the stack if present. If the target view is not part of the stack will not do anything.
        /// </summary>
        /// <param name="view">The view to go back to.</param>
        public virtual void PopUntil(View view)
        {
            View current = Peek();
            if (!stack.Contains(view))
            {
                UCGUILogger.LogWarning($"ViewStack does not contain {view}. Cannot go back to it!");
                return;
            }
            while (current != view && stack.Count != 0)
            {
                Pop();
                PopUntil(view);
                current = Peek();
            }
        }
        
        /// <summary>
        /// Pushes a view to the stack and opens it.
        /// </summary>
        /// <param name="view">The view to add to the stack.</param>
        ///
        /// <remarks>
        /// <i><see cref="View.OnStackJoined"/> is invoked on the view here to allow it to hold a reference to the stack it is in.</i>
        /// </remarks>
        public virtual void Push(View view)
        {
            if (!view)
                return;
            if (stack.Contains(view))
            {
                UCGUILogger.LogWarning($"ViewStack already contains {view}. Cannot forward to it again!");
                return;
            }
            
            stack.Push(view); 
            view.OnStackJoined(this).Open();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The top view in the stack without removing it.</returns>
        public View Peek()
        {
            return stack.TryPeek(out View top) ? top : null;
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