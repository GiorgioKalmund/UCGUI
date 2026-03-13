using System.ComponentModel;
using UnityEngine;

namespace UGUI
{
    public enum Direction
    {
        Left = (1 << 0),
        Right = (1 << 1),
        Up = (1 << 2),
        Down = (1 << 3)
        
    }

    public static class DirectionHelper
    {
        public static readonly Direction[] DirectionVectors = { Direction.Left, Direction.Right, Direction.Up, Direction.Down };

        public static Direction GetOpposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
            }

            throw new InvalidEnumArgumentException($"[Direction]: Invalid direction - {dir}");
        }
        
        
        public static Direction GetClockwise(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Left: return Direction.Up;
                case Direction.Right: return Direction.Down;
                case Direction.Up: return Direction.Right;
                case Direction.Down: return Direction.Left;
            }

            throw new InvalidEnumArgumentException($"[Direction]: Invalid direction - {dir}");
        }

        public static Direction GetCounterClockwise(this Direction dir)
        {
            return dir.GetClockwise().GetOpposite();
        }

        public static string Name(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Left: return "L";
                case Direction.Right: return "R";
                case Direction.Up: return "U";
                case Direction.Down: return "D";
            }

            throw new InvalidEnumArgumentException($"[Direction]: Invalid direction - {dir}");
        }
        
        public static Vector2 GetVector(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Left: return Vector2.left;
                case Direction.Right: return Vector2.right;
                case Direction.Up: return Vector2.up;
                case Direction.Down: return Vector2.down;
            }

            throw new InvalidEnumArgumentException($"[Direction]: Invalid direction - {dir}");
        }
    }
}
