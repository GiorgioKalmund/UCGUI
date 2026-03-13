using System;
using UGUI;

namespace UCGUI {
    using UnityEngine;
    
    
    public struct DirectionMap
    {
        public class Entry
        {
            internal IFocusable focus;
            internal BaseComponent gameObject;

            private Entry(BaseComponent obj, IFocusable focusable)
            {
                gameObject = obj;
                focus = focusable;
            }
            
            public Entry(IFocusable focusable) : this(focusable as BaseComponent ?? throw new ArgumentException($"[Entry]: {focusable} is not of type BaseComponent!"))
            {
                
            }
            
            public Entry(BaseComponent obj) : this(obj, obj as IFocusable ?? throw new ArgumentException($"[Entry]: {obj.name} is not of type IFocusable!"))
            {
                
            }
        }
        
        internal Entry left;
        internal Entry  right;
        internal Entry  up;
        internal Entry  down;

        public DirectionMap(Entry left, Entry right, Entry up, Entry down)
        {
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }

        public Entry Get(Direction forDirection)
        {
            switch (forDirection)
            {
                case Direction.Left: return left;
                case Direction.Right: return right;
                case Direction.Up: return up;
                case Direction.Down: return down;
            }

            return null;
        }


        public void Fill(Entry element)
        {
            left = element;
            right = element;
            up = element;
            down = element;
        }

        public void Assign(Entry element, Direction forDirection)
        {
            if (forDirection == Direction.Left)
                left = element;
            else if (forDirection == Direction.Right)
                right = element;
            else if (forDirection == Direction.Up)
                up = element;
            else if (forDirection == Direction.Down)
                down = element;
            else
            {
                UCGUILogger.LogError("[DirectionMap]: Could not assign field");
            }
        }

        public void Assign(Entry element, Vector2 forDirection)
        {
            if (forDirection == Vector2.left)
                left = element;
            else if (forDirection == Vector2.right)
                right = element;
            else if (forDirection == Vector2.up)
                up = element;
            else if (forDirection == Vector2.down)
                down = element;
            else
            {
                UCGUILogger.LogError("[DirectionMap]: Could not assign field");
            }
        }

        public override string ToString()
        {
            return
                $"(Left):{left?.gameObject.gameObject.name}\n" +
                $"(Right):{right?.gameObject.gameObject.name}\n" +
                $"(Up):{up?.gameObject.gameObject.name}\n" +
                $"(Down):{down?.gameObject.gameObject.name}\n";
        }
    }
}
