namespace UCGUI
{
    using UnityEngine;

    /// <summary>
    /// Inclusive floating point range.
    /// Allows to <see cref="Flip"/> the range as well as <see cref="EnsureOrdered"/>.
    /// </summary>
    [System.Serializable]
    public struct Range
    {
        public float minValue;
        public float maxValue;

        /// <summary>
        /// Value describing if the range is ordered, meaning that <see cref="minValue"/> is smaller or equal to <see cref="maxValue"/>.
        /// </summary>
        public bool IsOrdered => minValue <= maxValue;

        public Range(float min, float max)
        {
            minValue = min;
            maxValue = max;
        }

        /// <summary>
        /// Ensures that a range is ordered <seealso cref="IsOrdered"/>, meaning that <see cref="minValue"/> is smaller or equal to <see cref="maxValue"/>. If it is not, the values are flipped.
        /// </summary>
        public void EnsureOrdered()
        {
            if (!IsOrdered)
            {
                Flip();
            }
        }

        /// <summary>
        /// Swaps <see cref="minValue"/> and <see cref="maxValue"/>.
        /// </summary>
        public void Flip()
        {
            (minValue, maxValue) = (maxValue, minValue);
        }
        /// <summary>
        /// </summary>
        /// <returns>
        /// A new <see cref="Range"/> with flipped values.
        /// </returns>
        public Range Flipped()
        {
            return new Range(maxValue, minValue);
        }

        public static implicit operator Vector2(Range r) => new Vector2(r.minValue, r.maxValue);
        public static implicit operator Range(Vector2 v) => new Range(v.x, v.y);

        public override string ToString()
        {
            return $"[{minValue},{maxValue}]";
        }
    }
}