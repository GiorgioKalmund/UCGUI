using UnityEngine;

namespace UCGUI
{
    /// <summary>
    /// Position shorthands for pivots of RectTransforms.
    /// </summary>
    public enum PivotPosition 
    {
        UpperLeft, UpperCenter, UpperRight,
        MiddleLeft, MiddleCenter, MiddleRight, 
        LowerLeft, LowerCenter, LowerRight,
    }
    
    public static class PivotPositionHelper
    {
        public static Vector2 GetVector(this PivotPosition pos)
        {
            switch (pos)
            {
                case PivotPosition.UpperLeft: return new Vector2(0, 1);
                case PivotPosition.UpperCenter: return new Vector2(0.5f, 1);
                case PivotPosition.UpperRight: return new Vector2(1, 1);
                case PivotPosition.MiddleLeft: return new Vector2(0, 0.5f);
                case PivotPosition.MiddleCenter: return new Vector2(0.5f, 0.5f);
                case PivotPosition.MiddleRight: return new Vector2(1, 0.5f);
                case PivotPosition.LowerLeft: return new Vector2(0, 0);
                case PivotPosition.LowerCenter: return new Vector2(0.5f, 0);
                case PivotPosition.LowerRight: return new Vector2(1, 0);
                default:
                {
                    UCGUILogger.LogError($"Invalid PivotPosition {pos}!");
                    return new Vector2(0.5f, 0.5f);
                }
            }
        }
    }
}