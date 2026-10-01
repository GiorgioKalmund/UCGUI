using System;
using UnityEngine;

namespace UCGUI
{
    /// <summary>
    /// Object containing all individual frames, as well as the time every frame will show for. <i>Assuming <see cref="SpriteAnimator.speed"/> is 1</i>.
    /// <seealso cref="SpriteAnimator"/>
    /// </summary>
    /// <example>
    /// <code>
    /// // Every frame will appear for (1 / fps) seconds => here 0.1s each (assuming speed is 1).
    /// SpriteAnimation myAnimation1 = new SpriteAnimation(frameArray, 10);
    /// // Every frame potentially has its individual frame time, allowing for different pacing.
    /// SpriteAnimation myAnimation2 = new SpriteAnimation(frameArray, frameTimeArray);
    /// </code>
    /// </example>
    [Serializable]
    public class SpriteAnimation
    {
        public Sprite[] frames;
        public float[] framesPerSecond;

        public SpriteAnimation(Sprite[] frames, float[] framesPerSeconds)
        {
            this.frames = frames;
            framesPerSecond = framesPerSeconds;
        }

        public SpriteAnimation(Sprite[] frames, float framesPerSecond)
        {
            this.frames = frames;
            this.framesPerSecond = new float[frames.Length];
            Array.Fill(this.framesPerSecond, framesPerSecond);
        }
    }
}